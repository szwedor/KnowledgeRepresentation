using Stories.Graph;
using Stories.Graph.Model;
using Stories.Parser.Conditions;
using Stories.Parser.Statements;
using Stories.Parser.Statements.QueryStatements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return false;
        }

        private static bool ExecuteNecessarySufficiency(IEnumerable<Vertex> startVertices, IEnumerable<Vertex> endVertices)
        {
            var queryResult = true;
            var verticesToCheck = startVertices;
            var closedVertices = new List<Vertex>();
            do
            {
                if (!verticesToCheck.All(v => endVertices.Contains(v)))
                {
                    queryResult = false;
                    break;
                }
                closedVertices.AddRange(verticesToCheck);
                verticesToCheck = verticesToCheck.SelectMany(x => x.EdgesOutgoing.Select(y => y.To)).ToList();
            } while (!verticesToCheck.All(v => closedVertices.Contains(v)));

            return queryResult;
        }
    }
}
