using System.Linq;
using Sprache;
using Stories.Parser.Conditions;
using Stories.Parser.Statements;

namespace Stories.Parser.Parsers
{
    static class ReleaseStatementParser
    {
        public static readonly Parser<ReleaseStatement> Releases =
            (from agents in CommonParser.WhenAgents.Optional()
             from action in CommonParser.Action
             from releases in KeywordsParser.Releases
             from negation in KeywordsParser.Not.Optional()
             from fluent in CommonParser.Fluent
             from condition in CommonParser.IfCondition.Optional()
             select new ReleaseStatement
             {
                 Agents = agents.GetOrElse(null),
                 Action = action,
                 IsNegated = !negation.IsEmpty,
                 Fluent = fluent,
                 Condition = condition.GetOrElse(new ConditionConstant(true))
             }).Token();
    }
}
