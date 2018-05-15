using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Stories.Execution;
using Stories.Graph.Model;
using Stories.Parser.Statements;
using Stories.Parser.Statements.QueryStatements;

namespace Stories.Graph
{
    public class Graph
    {
        public List<Vertex> Vertexes;

        public List<Edge> Edges;

        private Graph()
        {
            this.Vertexes = new List<Vertex>();
            this.Edges = new List<Edge>();
        }

        public static Graph CreateGraph(Story story, QueryStatement query)
        {
            List<string> actors = new List<string>();
            if (query is AfterQueryStatement afterQuery)
            {
                actors = afterQuery.Actions.Select(p => p.Agent).ToList();
            }

            if (query is ExecutableQueryStatement executableQuery)
            {
                actors = executableQuery.Actions.Select(p => p.Agent).ToList();                
            }

            if (query is AgentInQueryStatement agentInQuery)
            {
                actors = agentInQuery.Actions.Select(p => p.Agent).ToList();
                if(!string.IsNullOrEmpty(agentInQuery.Agent) && !actors.Contains(agentInQuery.Agent))
                    actors.Add(agentInQuery.Agent);               
            }
            return Graph.CreateGraph(story, actors);
        }

        private static Graph CreateGraph(Story story, List<string> queryActors)
        {
            Graph graph = new Graph();
            foreach (var state in story.States)
            {
                graph.Vertexes.Add(new Vertex(state));
            }
            bool hasAnyActor = false;
            foreach (var vertexFrom in graph.Vertexes)
            {
                if (story.Agents.Count > 0)
                {
                    hasAnyActor = true;
                    foreach (var agent in story.Agents)
                    {
                        GetEdges(agent, graph, story, vertexFrom);
                    }
                }
          
                foreach (var actor in queryActors)
                {
                    if (!story.Agents.Contains(actor))
                    {
                        GetEdges(actor, graph, story, vertexFrom);
                        hasAnyActor = true;
                    }
                }

                if (!hasAnyActor)
                {
                    GetEdges(null, graph, story, vertexFrom);
                }

            }

            return graph;
        }

        private static void GetEdges(string agent, Graph graph, Story story, Vertex vertexFrom)
        {
            foreach (var action in story.Actions)
            {
                var resN = story.ResN(agent, action, vertexFrom.State);
                var resAb = story.ResAb(agent, action, vertexFrom.State);
                foreach (var state in resN.ToArray())
                {
                    var vertexTo = graph.Vertexes.Find(x => x.State.Equals(state));
                    if (graph.Edges.Any(p =>
                        p.From.State.Equals(vertexFrom.State) && p.To.State.Equals(vertexTo.State)
                        && p.Actor == agent && p.Action == action))
                        continue;
                    Edge edge = new Edge(vertexFrom, vertexTo, true, action, agent);
                    vertexFrom.EdgesOutgoing.Add(edge);
                    vertexTo.EdgesIncoming.Add(edge);
                    graph.Edges.Add(edge);
                }

                foreach (var state in resAb.ToArray())
                {
                    var vertexTo = graph.Vertexes.Find(x => x.State.Equals(state));
                    if (graph.Edges.Any(p =>
                        p.From.State.Equals(vertexFrom.State) && p.To.State.Equals(vertexTo.State)
                        && p.Actor == agent && p.Action == action))
                        continue;

                    Edge edge = new Edge(vertexFrom, vertexTo, false, action, agent);
                    vertexFrom.EdgesOutgoing.Add(edge);
                    vertexTo.EdgesIncoming.Add(edge);
                    graph.Edges.Add(edge);
                }
            }
        }

        public string GetString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var vertex in Vertexes)
            {
                sb.AppendLine(vertex.State.ToString());
            }

            foreach (var edge in Edges)
            {
                sb.AppendLine("From " + edge.From.State.ToString() 
                                      + "To " + edge.To.State.ToString()
                                      + "Action " + edge.Actor + " " + edge.Action + "Typically " + edge.IsTypical);
            }
            return sb.ToString();
        }
    }
}
