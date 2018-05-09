using Stories.Graph.Model;
using Stories.Parser.Conditions;
using Stories.Parser.Statements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stories.Graph
{
    public static class GraphExtensions
    {
        public static IEnumerable<Vertex> FindVerticesSatisfyingCondition(this Graph graph, ConditionExpression condition)
        {
            return graph.Vertexes.Where(v => v.State.EvaluateCondition(condition));
        }

        public static IEnumerable<Vertex> FindVerticesSatisfyingCondition(this IEnumerable<Vertex> vertices, ConditionExpression condition)
        {
            return vertices.Where(v => v.State.EvaluateCondition(condition));
        }

        public static IEnumerable<Vertex> ApplyInitiallyValueStatements(this IEnumerable<Vertex> startVertices, HistoryStatement history)
        {
            //value statements
            var initiallyValue = history.Values.Where(x => x.Actions.Count == 0);
            //apply initially x
            foreach (var val in initiallyValue.Where(x => x.IsObservable == false))
            {
                startVertices = startVertices.FindVerticesSatisfyingCondition(val.Condition);
            }
            //apply observable x
            startVertices = startVertices.Concat(initiallyValue.Where(x => x.IsObservable == true)
                .SelectMany(x => startVertices.FindVerticesSatisfyingCondition(x.Condition))).Distinct();

            return startVertices;
        }
    }
}
