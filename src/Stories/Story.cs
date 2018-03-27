using System;
using System.Collections.Generic;
using System.Linq;
using Stories.Parser.Conditions;
using Stories.Parser.Statements;

namespace Stories
{
    public class GraphNode
    {
        public AppState State { get; set; }
        public List<GraphEdge> Edges { get; set; }
    }

    public class GraphEdge
    {
        public string Action { get; set; }
        public string Agent { get; set; }
        public GraphNode Node { get; set; }
        public bool IsTypical { get; set; }
    }

    public class Graph
    {
        public List<GraphNode> Nodes { get; set; }
    }

    class Story
    {
        private HistoryStatement history { get; set; }
        private List<Fluent> fluents { get; set; }
        private List<string> agents { get; set; }
        private List<string> actions { get; set; }

        private List<AppState> states { get; set; }
        private Graph graph { get; set; }

        public Story(HistoryStatement history)
        {
            this.history = history;

            var fluents = getAllFluents();
            var nonInertialFluents = getAllNonInertialFluents();

            this.fluents = fluents.Select(f => new Fluent
            {
                Label = f,
                IsInertial = !nonInertialFluents.Contains(f)
            }).ToList();

            agents = getAllAgents();
            actions = getAllActions();

            states = AppState.ValidStates(history.Always.Select(a => a.Condition).ToList(), fluents.ToArray()).ToList();

            CreateGraph();
        }

        private void CreateGraph()
        {
            var nodes = states.ToDictionary(s => s, s => new GraphNode
            {
                State = s,
                Edges = new List<GraphEdge>()
            });

            graph = new Graph
            {
                Nodes = nodes.Values.ToList()
            };

            foreach (var node in graph.Nodes)
            {
                foreach (var effect in history.Effects)
                {
                    if (node.State.EvaluateCondition(effect.Condition))
                    {
                        var agents = effect.Agents ?? this.agents;

                        if (agents.Count > 0)
                        {
                            var nextNodes = states
                                .Where(s => s.EvaluateCondition(effect.Effect))
                                .Select(s => nodes[s])
                                .ToList();

                            foreach (var nextNode in nextNodes)
                            {
                                foreach (var agent in agents)
                                {
                                    node.Edges.Add(new GraphEdge
                                    {
                                        Action = effect.Action,
                                        Agent = agent,
                                        IsTypical = effect.IsTypical,
                                        Node = nextNode,
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        private List<string> getAllActions()
        {
            var effects = history.Effects.Select(e => e.Action);
            var releases = history.Releases.Select(r => r.Action);
            var values = history.Values.SelectMany(v => v.Actions).Select(a => a.Action);

            return effects
                .Concat(releases)
                .Concat(values)
                .Distinct()
                .ToList();
        }

        private List<string> getAllAgents()
        {
            var values = history.Values.SelectMany(v => v.Actions).Select(a => a.Agent);
            var effects = history.Effects.SelectMany(e => e.Agents ?? new List<string>());

            return effects.Where(a => !string.IsNullOrEmpty(a)).Distinct().ToList();
        }

        private List<string> getAllFluents()
        {
            var always = history.Always.SelectMany(a => getAllFluents(a.Condition));
            var effects = history.Effects.SelectMany(e => getAllFluents(e.Condition).Concat(getAllFluents(e.Effect)));
            var nonInertials = history.NonInertial.Select(n => n.Fluent);
            var releases = history.Releases.SelectMany(r => getAllFluents(r.Condition).Concat(new[] { r.Fluent }));
            var values = history.Values.SelectMany(v => getAllFluents(v.Condition));

            return always
                .Concat(effects)
                .Concat(nonInertials)
                .Concat(releases)
                .Concat(values)
                .Distinct()
                .ToList();
        }

        private List<string> getAllNonInertialFluents()
        {
            return history.NonInertial.Select(n => n.Fluent).Distinct().ToList();
        }

        private IEnumerable<string> getAllFluents(ConditionExpression expression)
        {
            switch (expression)
            {
                case ConditionConstant constant: return new string[0];
                case ConditionNegation negation: return getAllFluents(negation.Expression);
                case ConditionOperation operation:
                    var left = getAllFluents(operation.Left);
                    var right = getAllFluents(operation.Right);

                    return left.Concat(right);
                case ConditionVariable variable: return new[] { variable.Label };
                default: throw new InvalidOperationException();
            }
        }
    }
}
