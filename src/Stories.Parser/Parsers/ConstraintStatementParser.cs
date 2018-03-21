using System.Linq;
using Sprache;
using Stories.Parser.Conditions;
using Stories.Parser.Statements;

namespace Stories.Parser.Parsers
{
    static class ConstraintStatementParser
    {
        public static readonly Parser<AlwaysStatement> Always =
            (from always in KeywordsParser.Always
             from condition in ConditionsParsing.Condition
             select new AlwaysStatement
             {
                 Condition = condition
             }).Token();
    }
}
