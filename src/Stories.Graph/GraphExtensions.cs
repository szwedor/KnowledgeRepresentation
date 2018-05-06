using Stories.Graph.Model;
using Stories.Parser.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stories.Graph
{
    public static class GraphExtensions
    {
        public static IEnumerable<Vertex> FindVerticesSatisfyingCondition(this Graph graph, ConditionExpression condition)
        {
            var fluents = extractFluentsValues(condition);
            return graph.Vertexes.Where(v => fluents.All(f => v.State.Values[f.Key] == f.Value));
        }

        public static IEnumerable<Vertex> FindVerticesSatisfyingCondition(this IEnumerable<Vertex> vertices, ConditionExpression condition)
        {
            var fluents = extractFluentsValues(condition);
            return vertices.Where(v => fluents.All(f => v.State.Values[f.Key] == f.Value));
        }

        private static IEnumerable<KeyValuePair<string, bool>> extractFluentsValues(ConditionExpression condition)
        {
            switch (condition)
            {
                case ConditionConstant constant: return new Dictionary<string, bool>();
                case ConditionNegation negation: return extractFluentsValues(negation.Expression).Select(x => new KeyValuePair<string, bool>(x.Key, !x.Value));
                case ConditionOperation operation:
                    var left = extractFluentsValues(operation.Left);
                    var right = extractFluentsValues(operation.Right);

                    return left.Concat(right);
                case ConditionVariable variable: return new Dictionary<string, bool> { { variable.Label, true } };
                default: throw new InvalidOperationException();
            }
        }
    }
}
