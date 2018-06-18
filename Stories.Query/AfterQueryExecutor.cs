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
    public class AfterQueryExecutor:Executor<AfterQueryStatement>
    {
        public override bool Execute(AfterQueryStatement query, Graph.Graph graph, HistoryStatement history)
        {
            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }
            query.StateFromCondition = query.StateFromCondition ?? graph.GetStartCondition(history);

            switch (query.Sufficiency)
            {
                case Sufficiency.Necessary:
                    return NecessaryAfter(query, graph, history);
                case Sufficiency.Possibly:
                    return PossiblyAfter(query, graph, history);
                case Sufficiency.Typically:
                    return TypicallyAfter(query, graph, history);
            }
            return false;
        }

        private static bool NecessaryAfter(AfterQueryStatement query, Graph.Graph graph, HistoryStatement history)
        {
            var startVertices = graph.Vertexes.FindVerticesSatisfyingCondition(query.StateFromCondition).ToList();
            
            foreach(var startVertex in startVertices)
            {
                var possibleFinalVertices = FollowProgram(query.Actions, startVertex, 0);

                if (possibleFinalVertices.Count==0 || 
                    possibleFinalVertices==null || 
                    !possibleFinalVertices.All(v => v.State.EvaluateCondition(query.StateToCondition)))
                    return false;
            }
            return true;
        }

        private static bool PossiblyAfter(AfterQueryStatement query, Graph.Graph graph, HistoryStatement history)
        {
            var startVertices = graph.Vertexes.FindVerticesSatisfyingCondition(query.StateFromCondition).ToList();

            foreach (var startVertex in startVertices)
            {
                
                var possibleFinalVertices = FollowProgram(query.Actions, startVertex, 0);

                if (possibleFinalVertices.Any(v => v.State.EvaluateCondition(query.StateToCondition)))
                    return true;
            }
            return false;
        }

        private static bool TypicallyAfter(AfterQueryStatement query, Graph.Graph graph, HistoryStatement history)
        {
            var startVertices = graph.Vertexes.FindVerticesSatisfyingCondition(query.StateFromCondition).ToList();

            foreach (var startVertex in startVertices)
            {
                var possibleFinalVertices = FollowProgramTypically(query.Actions, startVertex, 0);

                if (possibleFinalVertices.Count == 0 ||
                    possibleFinalVertices == null || 
                    !possibleFinalVertices.All(v => v.State.EvaluateCondition(query.StateToCondition)))
                    return false;
            }
            return true;
        }

        private static List<Vertex> FollowProgram(List<ActionWithExecutor> actions, Vertex startVertex, int currentAction)
        {
            List<Vertex> results = new List<Vertex>();

            //przeszliśmy całą ścieżkę
            if (currentAction >= actions.Count)
            {
                results.Add(startVertex);
                return results;
            }

            //wykonaj jedna akcje
            var action = actions[currentAction];
            var possibleVertices = startVertex.EdgesOutgoing.Where(e => e.Action == action.Action && e.Actor == action.Agent).Select(e => e.To).ToList();
            currentAction++;

            foreach (var possibleVertex in possibleVertices)
            {
                results.AddRange(FollowProgram(actions, possibleVertex, currentAction));
            }

            return results;
        }

        private static List<Vertex> FollowProgramTypically(List<ActionWithExecutor> actions, Vertex startVertex, int currentAction)
        {
            List<Vertex> results = new List<Vertex>();

            //przeszliśmy całą ścieżkę
            if (currentAction >= actions.Count)
            {
                results.Add(startVertex);
                return results;
            }

            //wykonaj jedna akcje
            var action = actions[currentAction];
            var possibleVertices = startVertex.EdgesOutgoing.Where(e => e.Action == action.Action && e.Actor == action.Agent && e.IsTypical).Select(e => e.To).ToList();
            currentAction++;

            foreach (var possibleVertex in possibleVertices)
            {
                results.AddRange(FollowProgramTypically(actions, possibleVertex, currentAction));
            }

            return results;
        }
    }
}
