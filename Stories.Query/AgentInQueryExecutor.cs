using Stories.Execution;
using Stories.Graph;
using Stories.Graph.Model;
using Stories.Parser.Statements;
using Stories.Parser.Statements.QueryStatements;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stories.Query
{
    public static class AgentInQueryExecutor
    {
        public static bool Execute(this AgentInQueryStatement query, Stories.Graph.Graph graph, Stories.Execution.Story story, HistoryStatement history)
        {
            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }
            // czy występuje jawnie w ścieżce
            if (query.Actions.Any(p => p.Agent == query.Agent))
                return true;

            // poszukiwanie krawędzi null, którą tylko on może spełnić z warunków początkowych
            var vertices = graph.Vertexes.ToList();
            foreach (var val in history.Values.Where(x => x.Actions.Count == 0))
            {
                vertices = vertices.FindVerticesSatisfyingCondition(val.Condition).ToList();
            }
            var validation = new int[query.Actions.Count];
            vertices.ForEach(p => Search(p, query, validation, query.Actions.Count));

            if (query.Sufficiency == Sufficiency.Necessary)
            {
                if (validation[validation.Length-1] == 0 )
                return false;

                for (int i = 0; i < validation.Length - 1; i++)
                    if (validation[i] == validation[validation.Length - 1])
                        return true;
            }
            if(query.Sufficiency == Sufficiency.Possibly)
            {
                if (validation[validation.Length - 1] == 0)
                    return false;

                for (int i = 0; i < validation.Length - 1; i++)
                    if (validation[i]>0)
                        return true;
            }
 
            return false;
        }

        private static bool Search(Vertex vertex, AgentInQueryStatement query, int[] validation, int lvl)
        {
            if (lvl == 0)
            {
                // scieżka była zdefiniowana
                validation[0]++;
                return true;
            }
            lvl--;
            //wszystkie krawędzie z obecnego wierzchołka o danej akcji
            var actions = vertex.EdgesOutgoing
                .Where(p => p.Action == query.Actions[lvl].Action).ToArray();

            if (query.Actions[lvl].Agent != null)
            {
                //jesli w query akcja ma aktora to zwracamy czy ktorakolwiek sciezka ma koniec
                return actions.Where(p => p.Actor == query.Actions[lvl].Agent)
                     .Any(p => Search(p.To, query, validation, lvl));
            }
            else
            {
                if (actions.Length == 1 && actions[0].Actor == query.Agent)
                {
                    // w query nie ma aktora dla danej akcji i w grafie 
                    //jest tylko przejscie dla naszego agenta
                    if (Search(actions[0].To, query, validation, lvl))
                    {
                        validation[lvl]++;
                        return true;
                    }
                }
                else return actions.Where(p => p.Actor == query.Actions[lvl].Agent)
                   .Any(p => Search(p.To, query, validation, lvl));
            }
            return false;
        }
    }
}
