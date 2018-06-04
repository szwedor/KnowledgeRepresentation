using Stories.Execution;
using Stories.Graph;
using Stories.Graph.Model;
using Stories.Parser;
using Stories.Parser.Statements;
using Stories.Parser.Statements.QueryStatements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stories.Query
{
    public class AgentInQueryExecutor: Executor<AgentInQueryStatement>
    {
        public override bool Execute(AgentInQueryStatement query, Stories.Graph.Graph graph,  HistoryStatement history)
        {
            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }
            // czy występuje jawnie w ścieżce
            #region 
            var story = new Story(history);
            var nonExisitngActions = query.Actions.Select(p => p.Action).Except(story.Actions).ToArray();
            var actors = new HashSet<string>(story.Agents.Concat(query.Actions.Select(p => p.Agent).Concat(new[] { query.Agent })));
            actors.ExceptWith(new string[] { null });
            foreach (var actor in actors)
            foreach(var action in nonExisitngActions)
                foreach (var vertex in graph.Vertexes)
                {
                        var edge = new Edge(vertex, vertex, true, action, actor);
                        vertex.EdgesIncoming.Add(edge);
                        vertex.EdgesOutgoing.Add(edge);
                }
            #endregion
            //if (query.Actions.Any(p => p.Agent == query.Agent))
            //    return true;

            // poszukiwanie krawędzi null, którą tylko on może spełnić z warunków początkowych
            var vertices = graph.Vertexes.ToList();
            foreach (var val in history.Values.Where(x => x.Actions.Count == 0))
            {
                vertices = vertices.FindVerticesSatisfyingCondition(val.Condition).ToList();
            }
            query.Actions.Reverse();
            if (query.Sufficiency == Sufficiency.Necessary)
            {
                var search = new necessarySearch();

                 vertices.ForEach(p => search.Search(p, query, query.Actions.Count, new List<Edge>()));

                return search.started && search.result;
                
            }
            if(query.Sufficiency == Sufficiency.Possibly)
            {
                var search = new possibleSearch();
                vertices.ForEach(p => search.Search(p, query, query.Actions.Count, new List<Edge>()));

                return search.started && search.result;
            }

            return false;
        }
        public class necessarySearch
        {

            //            .Where(p => p.Action == query.Actions[lvl].Action && p.IsTypical).ToArray();
            public bool result = true;
            public bool started = false;
            public bool haveTypical = false;
            public void Search(Vertex vertex, AgentInQueryStatement query, int lvl, List<Edge> edges)
            {
                if (lvl == 0)
                {
                    started = result &= edges.Any(p => p.Actor == query.Agent);
                    haveTypical |= edges.Any(p => p.IsTypical);
                    return;
                }
                lvl--;
                var actions = vertex.EdgesOutgoing
                    .Where(p => p.Action == query.Actions[lvl].Action &&
                    (p.Actor == query.Actions[lvl].Agent || query.Actions[lvl].Agent == null))
                    .ToArray();
                if(!actions.Any())
                {
                    result = false;
                }
                foreach (var edge in actions)
                    Search(edge.To, query, lvl, edges.Concat(new[] { edge }).ToList());
            }
        }
        public class possibleSearch
        {
            
            public bool result = false;
            public bool started = false;
            public void Search(Vertex vertex, AgentInQueryStatement query, int lvl, List<Edge> edges)
            {
                if (lvl == 0) { 
                    started = result = result || (edges.Any(p => p.Actor == query.Agent));
                    return;
                 }
                lvl--;

                var actions = vertex.EdgesOutgoing
                    .Where(p => p.Action == query.Actions[lvl].Action &&
                    (p.Actor == query.Actions[lvl].Agent || query.Actions[lvl].Agent == null))
                    .ToArray();
                foreach (var edge in actions)
                    Search(edge.To, query, lvl, edges.Concat(new[] { edge }).ToList());
            }
        }
   
    }
}
