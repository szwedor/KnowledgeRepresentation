using Stories.Execution;
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

            var startVertices = graph.FindVerticesSatisfyingCondition(query.StateFromCondition).ToList();
            startVertices = startVertices.ApplyValueStatements(history.Values.Where(x => x.Actions.Count == 0)).ToList();

            switch (query.Sufficiency)
            {
                case Sufficiency.Necessary:
                    return ExecuteNecessarySufficiency(startVertices, query.StateToCondition, history);
                case Sufficiency.Possibly:
                    break;
                case Sufficiency.Typically:
                    return ExecuteTypicallySufficiency(startVertices, query.StateToCondition);
                default:
                    throw new InvalidOperationException();
            }
            return false;
        }

        private static bool ExecuteNecessarySufficiency(IEnumerable<Vertex> startVertices, ConditionExpression endCondition, HistoryStatement history)
        {
            // TODO do uwzględnienia zdania  "y after xyz"
            if (startVertices.Count() == 0)
            {
                return false;
            }

            var queryResult = true;
            int verticesCompleted = 0;

            foreach (var sVertex in startVertices)
            {
                var closedVertices = new SortedSet<Vertex>(Comparer<Vertex>.Create((x, y) => x.Equals(y) ? 0 : 1));
                var verticesToCheck = new List<List<Vertex>>() { new List<Vertex> { sVertex } };
                int programLength = 0;
                var possibleValueStatements = history.Values.Where(x=>x.Actions.Count>0);
                do
                {
                    // po kolei sprawdzamy grupy wierzchołków, do których prowadzi (aktor, akcja)
                    // jeśli dla jakiejś grupy wierzchołków, do której prowadzi (aktor, akcja) wszystkie stany spełniają warunek końcowy to wejdź w if'a
                    if (verticesToCheck.Any(x => x.All(y => y.State.EvaluateCondition(endCondition))))//endVertices.Contains(y))))
                    {
                        verticesCompleted++;
                        break;
                    }
                    // TODO: Czy w tym miejscu właściwe jest dorzucanie wierzchołków do closedVertices jeśli później verticesToCheck obetniemy o valueStatements?
                    closedVertices.UnionWith(verticesToCheck.SelectMany(x => x).Distinct());

                    // grupujemy krawędzie po aktorach i akcjach
                    var edgesGroupedByActorAction = verticesToCheck.SelectMany(x => x).Distinct().SelectMany(x => x.EdgesOutgoing).GroupBy(y => new { y.Action, y.Actor }).ToList();
                    // jedna pozycja w liście to zbiór wierzchołków, do których możemy się dostać po wykonaniu (aktor, akcja)
                    verticesToCheck = edgesGroupedByActorAction.Select(x => x.Select(y=>y.To).ToList()).ToList();

                    // TODO odblokowac po sprawdzeniu poprawnosci bez uwzgledniania valueStatements
                    //////////////////////
                    // wyliczamy możliwe teraz i w przyszłości valueStatements
                    possibleValueStatements = possibleValueStatements.Where(x => edgesGroupedByActorAction.Any(
                            y => y.Key.Action == x.Actions[programLength].Action
                            && y.Key.Actor == x.Actions[programLength].Agent)
                        ).ToList();
                    programLength++;

                    // wyznaczamy valueStatements dla których aktualnie spełniamy program
                    var valuesStatementToApply = possibleValueStatements.Where(x => x.Actions.Count == programLength).ToList();
                    if (valuesStatementToApply.Count > 0)
                    {
                        //////var anyChangesInVerticesToCheck = 
                        //////var anyChangesInVerticesToCheck = !valuesStatementToApply.All( // verticesToCheck == verticesToCheck po nałożeniu wszystkich valueStatements
                        //////      x => verticesToCheck.All( // vertivesToCheck == verticesToCheck po nałożeniu ograniczeń z Condition
                        //////          y => y.All(z => y.FindVerticesSatisfyingCondition(x.Condition).Contains(z) // y == y po nałożeniu ograniczeń z Condition
                        //////      ))
                        //////  );
                        // TODO co zrobić jeśli nie wszystkei wyznaczone stany mogą iść dalej??
                        // czy to oznacza, ze dla danej drogi nie możemy uzyskać efektu??
                        // a może należy obciąć verticesToCheck o właściwe stany i procesować dalej? - TA ODPOWIEDZ JEST CHYBA PRAWDZIWA

                        // obciecie verticesToCheck o stany niespelniajace ValueStatements
                        //valuesStatementToApply.ForEach(
                        //    x =>
                        //    {
                        //        verticesToCheck = verticesToCheck.Select(
                        //            y => y.FindVerticesSatisfyingCondition(x.Condition).ToList()).ToList();
                        //    });
                    }

                    // sprawdzamy, czy wszystkie wierzchołki do rozważenia były już rozważane
                } while (!verticesToCheck.SelectMany(x => x.ToList()).All(v => closedVertices.Contains(v)));
            }

            if (verticesCompleted != startVertices.Count())
            {
                queryResult = false;
            }

            return queryResult;
        }

        private static bool ExecuteTypicallySufficiency(IEnumerable<Vertex> startVertices, ConditionExpression endCondition)
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
