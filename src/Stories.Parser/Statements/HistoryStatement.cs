using System.Collections.Generic;
using System.Linq;

namespace Stories.Parser.Statements
{
    public class HistoryStatement
    {
        public List<EffectStatement> Effects { get; set; }
        public List<AlwaysStatement> Always { get; set; }
        public List<NonInertialStatement> NonInertial { get; set; }
        public List<ReleaseStatement> Releases { get; set; }
        public List<ValueStatement> Values { get; set; }

        public HistoryStatement(IEnumerable<Statement> statements)
        {
            Effects = statements.OfType<EffectStatement>().ToList();
            Always = statements.OfType<AlwaysStatement>().ToList();
            NonInertial = statements.OfType<NonInertialStatement>().ToList();
            Releases = statements.OfType<ReleaseStatement>().ToList();
            Values = statements.OfType<ValueStatement>().ToList();

            foreach (var effect in Effects.Where(p => p.IsTypical).ToArray())
                Effects.Add(new EffectStatement()
                {
                    Effect = new Conditions.ConditionConstant(true),
                    Action = effect.Action,
                    Agents = effect.Agents,
                    Condition = effect.Condition,
                    IsTypical = false
                });
        }
    }
}
