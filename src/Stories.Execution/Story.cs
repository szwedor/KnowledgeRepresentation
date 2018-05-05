using System;
using System.Collections.Generic;
using System.Linq;
using Stories.Parser.Conditions;
using Stories.Parser.Statements;

namespace Stories.Execution
{
    public class Story
    {
        private HistoryStatement history { get;  }
        public IReadOnlyList<Fluent> Fluents { get; }
        public IReadOnlyList<string> Agents { get;  }
        public IReadOnlyList<string> Actions { get; }
        public IReadOnlyList<AppState> States { get; }

        public Story(HistoryStatement history)
        {
            this.history = history;
            var fluents = getAllFluents();
            var nonInertialFluents = getAllNonInertialFluents();

            this.Fluents = fluents.Select(f => new Fluent
            {
                Label = f,
                IsInertial = !nonInertialFluents.Contains(f)
            }).ToList();

            this.Agents = getAllAgents();
            this.Actions = getAllActions();

            this.States = AppState.ValidStates(history.Always.Select(a => a.Condition).ToList(), fluents.ToArray()).ToList();
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

        public HashSet<AppState> Res0(string agent, string action, AppState state)
        {
            var effects = history.Effects
                .Where(p => p.IsTypical == false)
                .Where(p=>p.Action == action)
                .Where(p => p.Agents == null || p.Agents.Contains(agent))
                .Where(p => state.EvaluateCondition(p.Condition)).ToList();
            if (!effects.Any()) return new HashSet<AppState>();
            var states = States.Where(p => effects.All(c => p.EvaluateCondition(c.Effect)));
            return new HashSet<AppState>(states);
        }

        public HashSet<string> New(string agent, string action, AppState state,AppState state2)
        {
            var fluent = Fluents.Where(p => p.IsInertial)
                .Where(p => state.GetVariable(p.Label) != state2.GetVariable(p.Label))
                .Select(p=>p.Label).ToList();

            var releasesFluent = history.Releases
                .Where(p => p.Agents == null || p.Agents.Contains(agent))
                .Where(p => state.EvaluateCondition(p.Condition))
                .Select(p => p.Fluent).ToList();

            return new HashSet<string>(fluent.Union(releasesFluent));
        }

        public HashSet<AppState> ResMinus(string agent, string action, AppState state)
        {
            var res = Res0(agent, action, state)
                .Select(p => new {state = p, @new = New(agent, action, state, p).Count})
                .OrderBy(p=>p.@new).ToList();
            return new HashSet<AppState>(res.Where(p=>p.@new==res[0].@new).Select(p=>p.state));
        }

        public HashSet<AppState> Res0Plus(string agent, string action, AppState state)
        {
            var actionEffects = history.Effects
                .Where(p => p.Action == action).ToList();

            var effects = actionEffects
                .Where(p => p.Agents == null || p.Agents.Contains(agent))
                .Where(p => state.EvaluateCondition(p.Condition)).ToList();

            var isCertainEffectOnly = effects.All(p => !p.IsTypical);
            if (!isCertainEffectOnly)
                effects = effects.Where(p => p.IsTypical).ToList();

            if (!effects.Any())
                return new HashSet<AppState>();

            var states = States
                .Where(p => effects.All(c => p.EvaluateCondition(c.Effect)))
                .Intersect(Res0(agent, action, state))
                .ToList();
            
            return new HashSet<AppState>(states);
        }
        public HashSet<AppState> ResN(string agent, string action, AppState state)
        {
            var res = this.Res0Plus(agent, action, state)
                .Select(p => new { state = p, @new = New(agent, action, state, p).Count })
                .OrderBy(p => p.@new).ToList();
            return new HashSet<AppState>(res.Where(p => p.@new == res[0].@new).Select(p => p.state));
          }
        public HashSet<AppState> ResAb(string agent, string action, AppState state)
        {
            var set = ResMinus(agent, action, state);
            set.ExceptWith(ResN(agent, action, state));
            return set;
        }
    }
}
