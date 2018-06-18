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
             from conditionFrom in CommonParser.FromCondition.Optional()
             select new AccessibleQueryStatement
             {
                 Sufficiency = sufficiency,
                 StateToCondition = conditionTo,
                 StateFromCondition = conditionFrom.GetOrDefault()
             }
            ).Token();
    }
}
