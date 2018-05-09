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

        public static IEnumerable<Vertex> ApplyValueStatements(this IEnumerable<Vertex> startVertices, IEnumerable<ValueStatement> valueStatements)
        {
            //apply value statements
            foreach (var val in valueStatements.Where(x => x.IsObservable == false))
            {
                startVertices = startVertices.FindVerticesSatisfyingCondition(val.Condition);
            }
            //apply observable value statements
            // to jest bez sensu, observable bez condition nie jest w stanie ani obciąć, ani rozszerzyć zbioru stanów
            startVertices = startVertices.Concat(valueStatements.Where(x => x.IsObservable == true)
                .SelectMany(x => startVertices.FindVerticesSatisfyingCondition(x.Condition))).Distinct();

            return startVertices;
        }
    }
}
