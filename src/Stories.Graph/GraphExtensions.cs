using Stories.Graph.Model;
using Stories.Parser.Conditions;
using System.Collections.Generic;
using System.Linq;

namespace Stories.Graph
{
    public static class GraphExtensions
    {
        public static IEnumerable<Vertex> FindVerticesSatisfyingCondition(this IEnumerable<Vertex> vertices, ConditionExpression condition)
        {
            return vertices.Where(v => v.State.EvaluateCondition(condition));
        }
    }
}
