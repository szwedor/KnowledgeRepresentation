using Stories.Graph;
using Stories.Graph.Model;
using Stories.Parser.Statements;
using Stories.Parser.Statements.QueryStatements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stories.Query
{
    public static class AccessibleQueryExecutor
    {
        public static bool Execute(this AccessibleQueryStatement query, Graph.Graph graph)
        {
            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            var startVertices = graph.FindVerticesSatisfyingCondition(query.StateFromCondition).ToList();
            var endVertices = graph.FindVerticesSatisfyingCondition(query.StateToCondition).ToList();

            switch (query.Sufficiency)
            {
                case Sufficiency.Necessary:
                    return ExecuteNecessarySufficiency(startVertices, endVertices);
                case Sufficiency.Possibly:
                    break;
                case Sufficiency.Typically:
                    return ExecuteTypicallySufficiency(startVertices, endVertices);
                default:
                    throw new InvalidOperationException();
            }
            return false;
        }

        private static bool ExecuteNecessarySufficiency(IEnumerable<Vertex> startVertices, IEnumerable<Vertex> endVertices)
        {
            // TODO do uwzględnienia zdania  "y after xyz"
            if (endVertices.Count() == 0 || startVertices.Count() == 0)
            {
                return false;
            }

            var queryResult = true;
            int verticesCompleted = 0;

            foreach (var sVertex in startVertices)
            {
                var closedVertices = new SortedSet<Vertex>(Comparer<Vertex>.Create((x, y) => x.Equals(y) ? 0 : 1));
                var verticesToCheck = new List<List<Vertex>>() { new List<Vertex> { sVertex } };

                do
                {
                    // po kolei sprawdzamy grupy wierzchołków, do których prowadzi (aktor, akcja)
                    // jeśli dla jakiejś grupy wierzchołków, do której prowadzi (aktor, akcja) wszystkie stany spełniają warunek końcowy to wejdź w if'a
                    if (verticesToCheck.Any(x => x.All(y => endVertices.Contains(y))))
                    {
                        verticesCompleted++;
                        break;
                    }
                    
                    closedVertices.UnionWith(verticesToCheck.SelectMany(x => x).Distinct());

                    // grupujemy krawędzie po aktorach i akcjach
                    var edgesGroupedByActorAction = verticesToCheck.SelectMany(x => x).Distinct().SelectMany(x => x.EdgesOutgoing).GroupBy(y => new { y.Action, y.Actor }).ToList();
                    // jedna pozycja w liście to zbiór wierzchołków, do których możemy się dostać po wykonaniu (aktor, akcja)
                    verticesToCheck = edgesGroupedByActorAction.Select(x => x.Select(y=>y.To).ToList()).ToList();

                    // sprawdzamy, czy wszystkie wierzchołki do rozważenia były już rozważane
                } while (!verticesToCheck.SelectMany(x => x.ToList()).All(v => closedVertices.Contains(v)));
            }

            if (verticesCompleted != startVertices.Count())
            {
                queryResult = false;
            }

            return queryResult;
        }

        private static bool ExecuteTypicallySufficiency(IEnumerable<Vertex> startVertices, IEnumerable<Vertex> endVertices)
        {
            throw new NotImplementedException();
            //var queryResult = true;
            //var verticesToCheck = startVertices;
            //var closedVertices = new SortedSet<Vertex>(Comparer<Vertex>.Create((x, y) => x.Equals(y) ? 0 : 1));
            //do
            //{
            //    if (!verticesToCheck.Any(v => endVertices.Contains(v)))
            //    {
            //        queryResult = false;
            //        break;
            //    }
            //    closedVertices.UnionWith(verticesToCheck);
            //    var typicalVertices = verticesToCheck.SelectMany(x => x.EdgesOutgoing.Where(y => y.IsTypical).Select(y => y.To)).Distinct().ToList();
            //    if (typicalVertices.Count > 0)
            //    {
            //        verticesToCheck = typicalVertices.Except(closedVertices).ToList();
            //    }
            //    else
            //    {
            //        verticesToCheck = verticesToCheck.SelectMany(x => x.EdgesOutgoing.Select(y => y.To)).Distinct().Except(closedVertices).ToList();
            //    }

            //} while (!verticesToCheck.All(v => closedVertices.Contains(v)));

            //return queryResult;
        }
    }
}
