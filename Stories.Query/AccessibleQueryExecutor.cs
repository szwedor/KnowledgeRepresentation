using Stories.Graph;
using Stories.Graph.Model;
using Stories.Parser.Conditions;
using Stories.Parser.Statements;
using Stories.Parser.Statements.QueryStatements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stories.Query
{
    public static class AccessibleQueryExecutor
    {
        public static bool Execute(this AccessibleQueryStatement query, Graph.Graph graph, HistoryStatement history)
        {
            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            var startVertices = graph.Vertexes.FindVerticesSatisfyingCondition(query.StateFromCondition).ToList();
            startVertices = BuildGraph(startVertices, graph, history).ToList();

            switch (query.Sufficiency)
            {
                case Sufficiency.Necessary:
                    return ExecuteNecessarySufficiency(startVertices, query.StateToCondition);
                case Sufficiency.Possibly:
                    break;
                case Sufficiency.Typically:
                    return ExecuteTypicallySufficiency(startVertices, query.StateToCondition);
                default:
                    throw new InvalidOperationException();
            }
            return false;
        }

        #region BuildGraph
        private static IEnumerable<Vertex> BuildGraph(IEnumerable<Vertex> startVertices, Graph.Graph graph, HistoryStatement history)
        {
            var vertices = startVertices;

            //apply initially value statements
            foreach (var val in history.Values.Where(x => x.Actions.Count == 0))
            {
                vertices = vertices.FindVerticesSatisfyingCondition(val.Condition).ToList();
            }

            vertices = DropUnreachableEdges(vertices);

            // zaaplikowanie value statements
            vertices = ApplyNotInitiallyValueStatements(graph, history, vertices);

            //zaaplikowanie zdan releases
            vertices = ApplyReleasesStatements(graph, history, vertices);
            return vertices;
        }

        private static IEnumerable<Vertex> DropUnreachableEdges(IEnumerable<Vertex> vertices)
        {

            // określenie wierzchołków, do których da sie dotrzeć
            var reachableVertices = new SortedSet<Vertex>(Comparer<Vertex>.Create((x, y) => x.State.Equals(y.State) ? 0 : 1));
            IEnumerable<Vertex> verticesToCheck = vertices;
            do
            {
                reachableVertices.UnionWith(verticesToCheck);
                verticesToCheck = verticesToCheck.SelectMany(x => x.EdgesOutgoing.Select(y => y.To)).Except(reachableVertices).ToList();

            } while (verticesToCheck.Count() != 0);

            // obcięcie krawędzie wchodzących do tych, którymi faktycznie możemy wejść do wierzchołka w tym query
            foreach (var vertex in reachableVertices)
            {
                vertex.EdgesIncoming = vertex.EdgesIncoming.Where(x => reachableVertices.Select(y => y.State).Contains(x.From.State)).ToList();
            }

            return vertices;
        }

        private static IEnumerable<Vertex> ApplyNotInitiallyValueStatements(Graph.Graph graph, HistoryStatement history, IEnumerable<Vertex> vertices)
        {
            foreach (var val in history.Values.Where(x => x.Actions.Count != 0))
            {
                var valuePaths = vertices.SelectMany(x => x.EdgesOutgoing);
                for (int i = 0; i < val.Actions.Count - 1; i++)
                {
                    // wybranie wszystkich krawędzi odpowiadajacych danej parze (actor, action) z value staatement
                    valuePaths = valuePaths.Where(y => y.Action == val.Actions[i].Action && y.Actor == val.Actions[i].Agent).Select(x => x.To)
                        .SelectMany(x => x.EdgesOutgoing);
                    if (valuePaths.Count() == 0)
                    {
                        break;
                    }
                }
                // krawędzie spełniajace warunek ostatniej pary (actor, action) z value staatement
                valuePaths = valuePaths.Where(y => y.Action == val.Actions[val.Actions.Count - 1].Action && y.Actor == val.Actions[val.Actions.Count - 1].Agent);

                var edgeList = valuePaths.ToList();
                for (int i = 0; i < edgeList.Count; i++)
                {
                    // znalezienie wierzchołka, do którego przeniesiemy się po zaaplikowaniu value statements
                    var vertexState = edgeList[i].To.State.Values.ToDictionary(x => x.Key, x => x.Value);
                    foreach (var fluent in val.Condition.ExtractFluentsValues())
                    {
                        vertexState[fluent.Key] = fluent.Value;
                    }
                    var vertexEvaluatingValueStatement = graph.Vertexes.Where(v => v.State.Values.All(x => vertexState[x.Key] == x.Value)).ToList();
                    if (vertexEvaluatingValueStatement.Count > 1)
                    {
                        throw new InvalidOperationException("vertexEvaluatingValueStatement length is grater than 1.");
                    }

                    // sprawdzić czy wszystkie drogi prowadzące do wierzchołka spełniają value statement
                    // TODO
                    if (edgeList[i].To.EdgesIncoming.All(x => valuePaths.Contains(x)) && val.IsObservable == false)
                    {
                        edgeList[i].To = vertexEvaluatingValueStatement.First();
                    }
                    else
                    {
                        vertexEvaluatingValueStatement.ForEach(
                                x =>
                                {
                                    var actionEdge = edgeList[i].From.EdgesOutgoing.FirstOrDefault(
                                        y => y.IsTypical == edgeList[i].IsTypical && edgeList[i].Action == y.Action && edgeList[i].Actor == y.Actor);
                                    if (actionEdge == null)
                                    {
                                        edgeList[i].From.EdgesOutgoing.Add(new Edge(edgeList[i].From, x, edgeList[i].IsTypical, edgeList[i].Action, edgeList[i].Actor));
                                    }
                                    else
                                    {
                                        actionEdge.To = x;
                                    }
                                });
                    }
                }
            }
            return vertices;
        }

        private static IEnumerable<Vertex> ApplyReleasesStatements(Graph.Graph graph, HistoryStatement history, IEnumerable<Vertex> vertices)
        {
            foreach (var release in history.Releases)
            {
                var edges = vertices.SelectMany(x => x.EdgesOutgoing.Where(y => y.Action == release.Action &&
                    (release.Agents == null || release.Agents.Contains(y.Actor)))).ToList();

                for(int i=0;i<edges.Count;i++)
                {
                    var edge = edges[i];
                    if (edge.From.State.EvaluateCondition(release.Condition))
                    {
                        var state = edge.To.State.Values.ToDictionary(x => x.Key, x => x.Value);
                        state[release.Fluent] = !state[release.Fluent];
                        var vertex = graph.Vertexes.FirstOrDefault(v => v.State.Values.All(x => state[x.Key] == x.Value));
                        if(vertex != null)
                        {
                            edge.From.EdgesOutgoing.Add(new Edge(edge.From, vertex, edge.IsTypical, edge.Action, edge.Actor));
                        }
                    }
                }
            }
            return vertices;
        }
        #endregion

        private static bool ExecuteNecessarySufficiency(IList<Vertex> startVertices, ConditionExpression endCondition)
        {
            if (startVertices.Count() == 0)
            {
                return false;
            }

            int verticesCompleted = 0;

            for (int m = 0; m < startVertices.Count; m++)
            {
                if (verticesCompleted < m)
                {
                    return false;
                }

                var closedVertices = new SortedSet<Vertex>(Comparer<Vertex>.Create((x, y) => x.State.Values.All(z => y.State.Values[z.Key] == z.Value) ? 0 : 1));
                var verticesToCheck = new List<List<Vertex>>() { new List<Vertex> { startVertices[m] } };
                do
                {
                    // sprawdzamy czy wszystkie wierzchołki, do których możemy dojść przy pomocy (akcja, aktor) spełniają warunek końcowy
                    if (verticesToCheck.Any(x => x.All(y => y.State.EvaluateCondition(endCondition))))
                    {
                        verticesCompleted++;
                        break;
                    }

                    closedVertices.UnionWith(verticesToCheck.SelectMany(x => x));

                    // grupujemy krawędzie po aktorach i akcjach
                    var edgesGroupedByActorAction = verticesToCheck.Select(z => z.SelectMany(x => x.EdgesOutgoing).GroupBy(y => new { y.Action, y.Actor })).SelectMany(x => x).ToList();
                    // jedna pozycja w liście to zbiór wierzchołków, do których możemy się dostać po wykonaniu (aktor, akcja)
                    verticesToCheck = edgesGroupedByActorAction.Select(x => x.Select(y => y.To).ToList()).ToList();

                    // sprawdzamy, czy wszystkie wierzchołki do rozważenia były już rozważane
                } while (!verticesToCheck.SelectMany(x => x.ToList()).All(v => closedVertices.Contains(v)));
            }

            if (verticesCompleted < startVertices.Count)
            {
                return false;
            }

            return true;
        }


        internal class AbnormalLengthPath
        {
            public int Abnormal { get; set; }
            public int Length { get; set; }
            public Vertex Vertex { get; set; }
        }

        private static bool ExecuteTypicallySufficiency(IEnumerable<Vertex> startVertices, ConditionExpression endCondition)
        {
            if (startVertices.Count() == 0)
            {
                return false;
            }

            var queryResult = true;
            int verticesCompleted = 0;

            foreach (var sVertex in startVertices)
            {
                var closedVertices = new SortedSet<Vertex>(Comparer<Vertex>.Create((x, y) => x.State.Values.All(z => y.State.Values[z.Key] == z.Value) ? 0 : 1));
                var verticesToCheck = new List<List<AbnormalLengthPath>>
                { new List<AbnormalLengthPath>{
                    new AbnormalLengthPath{Vertex = sVertex, Abnormal = 0, Length = 0 }
                }
                };

                do
                {
                    // grupujemy po długości trasy i w ramach tras o jednej długości szukamy tej o najmniejszej liczbie abnormalnych przejść
                    var groupedByLength = verticesToCheck.Select(x => x.GroupBy(y => y.Length)).ToList();
                    if (groupedByLength.Any(x => x.Any(y => y.OrderBy(z => z.Abnormal).First().Vertex.State.EvaluateCondition(endCondition))))
                    {
                        verticesCompleted++;
                        break;
                    }

                    closedVertices.UnionWith(verticesToCheck.SelectMany(x => x.Select(y => y.Vertex)));

                    // grupujemy krawędzie po aktorach i akcjach
                    var edgesGroupedByActorAction = verticesToCheck.Select(
                      z => z.SelectMany(x => x.Vertex.EdgesOutgoing.Select(y => new KeyValuePair<AbnormalLengthPath, Edge>(x, y))).GroupBy(y => new { y.Value.Action, y.Value.Actor }))
                      .SelectMany(x => x).ToList();
                    //var edgesGroupedByActorAction = verticesToCheck.SelectMany(x => x).SelectMany(x => x.Vertex.EdgesOutgoing.Select(y => new KeyValuePair<AbnormalLengthPath, Edge>(x, y))).GroupBy(y => new { y.Value.Action, y.Value.Actor }).ToList();
                    // jedna pozycja w liście to zbiór wierzchołków, do których możemy się dostać po wykonaniu (aktor, akcja)
                    // liczymy długość trasy jaką się dostaliśmy oraz licznę abnormalnych przejść
                    verticesToCheck = edgesGroupedByActorAction.Select(
                        y => y.Select(x => new AbnormalLengthPath { Vertex = x.Value.To, Abnormal = x.Key.Abnormal + Convert.ToInt32(!x.Value.IsTypical), Length = x.Key.Length + 1 }
                        ).ToList()).ToList();

                    // sprawdzamy, czy wszystkie wierzchołki do rozważenia były już rozważane
                } while (!verticesToCheck.SelectMany(x => x.ToList()).All(v => closedVertices.Contains(v.Vertex)));
            }

            if (verticesCompleted != startVertices.Count())
            {
                queryResult = false;
            }

            return queryResult;
        }
    }
}
