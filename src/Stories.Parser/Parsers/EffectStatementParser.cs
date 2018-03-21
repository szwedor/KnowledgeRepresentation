using System.Linq;
using Sprache;
using Stories.Parser.Conditions;
using Stories.Parser.Statements;

namespace Stories.Parser.Parsers
{
    static class EffectStatementParser
    {
        public static readonly Parser<EffectStatement> Causes =
            (from agents in CommonParser.WhenAgents.Optional()
             from action in CommonParser.Action
             from typically in KeywordsParser.Typically.Optional()
             from causes in KeywordsParser.Causes
             from effect in ConditionsParsing.Condition
             from condition in CommonParser.IfCondition.Optional()
             select new EffectStatement
             {
                 Agents = agents.GetOrElse(null),
                 Action = action,
                 IsTypical = !typically.IsEmpty,
                 Effect = effect,
                 Condition = condition.GetOrElse(new ConditionConstant(true))
             }).Token();

        public static readonly Parser<EffectStatement> Impossible =
            (from impossible in KeywordsParser.Impossible
             from agents in CommonParser.Agents.Optional()
             from action in CommonParser.Action
             from condition in CommonParser.IfCondition.Optional()
             select new EffectStatement
             {
                 Agents = agents.GetOrElse(null),
                 Action = action,
                 IsTypical = false,
                 Effect = new ConditionConstant(false),
                 Condition = condition.GetOrElse(new ConditionConstant(true))
             }).Token();
    }
}
