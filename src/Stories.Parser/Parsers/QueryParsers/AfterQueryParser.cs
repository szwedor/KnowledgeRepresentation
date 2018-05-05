using System.Linq;
using Sprache;
using Stories.Parser.Conditions;
using Stories.Parser.Statements.QueryStatements;

namespace Stories.Parser.Parsers.QueryParsers
{
    public static class AfterQueryParser
    {
        public static readonly Parser<AfterQueryStatement> AfterQuery =
            (from sufficiency in SufficiencyParser.Sufficiency
             from conditionTo in ConditionsParsing.Condition
             from after in KeywordsParser.After
             from actions in Parse.DelimitedBy(CommonParser.ActionWithExecutor, Parse.String(","))
             from fromKeyword in KeywordsParser.From
             from conditionFrom in ConditionsParsing.Condition
             select new AfterQueryStatement
             {
                 Sufficiency = sufficiency,
                 Actions = actions.ToList(),
                 StateToCondition = conditionTo,
                 StateFromCondition = conditionFrom
             }
            ).Token();
    }
}
