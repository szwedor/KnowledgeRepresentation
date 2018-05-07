using System.Collections.Generic;
using System.Linq;
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
            Graph graph = new Graph();
            foreach (var state in story.States)
            {
                graph.Vertexes.Add(new Vertex(state));
            }
            foreach (var vertexFrom in graph.Vertexes)
            {
                if (story.Agents.Count > 0)
                {
                    foreach (var agent in story.Agents)
                    {
                        GetEdges(agent, graph, story, vertexFrom);
                    }
                }

                if (query != null && query is AgentInQueryStatement agentQuery && !story.Agents.Contains(agentQuery.Agent))
                {
                    GetEdges(agentQuery.Agent, graph, story, vertexFrom);
                }

                if (query == null && story.Agents.Count == 0)
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
                sb.AppendLine("From " + edge.From.State.Values.Select(p => $"[{p.Key}:{p.Value}]") 
                                      + "To " + edge.To.State.Values.Select(p => $"[{p.Key}:{p.Value}]")
                                      + "Action " +edge.Actor + " " + edge.Action + "Typically " + edge.IsTypical);
            }
            return sb.ToString();
        }
    }
}
