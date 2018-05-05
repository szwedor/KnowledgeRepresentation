using System.Linq;
using Sprache;
using Stories.Parser.Conditions;
using Stories.Parser.Statements.QueryStatements;

namespace Stories.Parser.Parsers.QueryParsers
{
    public static class AccessibleQueryParser
    {
        public static readonly Parser<AccessibleQueryStatement> AccessibleQuery =
            (from sufficiency in SufficiencyParser.Sufficiency
             from accessible in KeywordsParser.Accessible
             from conditionTo in ConditionsParsing.Condition
             from fromKeyword in KeywordsParser.From
             from conditionFrom in ConditionsParsing.Condition
             select new AccessibleQueryStatement
             {
                 Sufficiency = sufficiency,
                 StateToCondition = conditionTo,
                 StateFromCondition = conditionFrom
             }
            ).Token();
    }
}
