using System.Collections.Generic;
using System.Linq;
using Sprache;
using Stories.Parser.Conditions;
using Stories.Parser.Statements;

namespace Stories.Parser.Parsers
{
    static class ValueStatementParser
    {
        public static readonly Parser<ValueStatement> After =
            (from observable in KeywordsParser.Observable.Optional().Select(v => !v.IsEmpty)
             from condition in ConditionsParsing.Condition
             from after in KeywordsParser.After
             from actions in Parse.DelimitedBy(CommonParser.ActionWithExecutor, Parse.String(","))
             select new ValueStatement
             {
                 IsObservable = observable,
                 Condition = condition,
                 Actions = actions.ToList()
             }).Token();

        public static readonly Parser<ValueStatement> Initial =
            (from initially in KeywordsParser.Initially
             from condition in ConditionsParsing.Condition
             select new ValueStatement
             {
                 IsObservable = false,
                 Condition = condition,
                 Actions = new List<ActionWithExecutor>()
             }).Token();
    }
}
