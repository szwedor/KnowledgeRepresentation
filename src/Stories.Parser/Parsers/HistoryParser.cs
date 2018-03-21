using System.Linq;
using Sprache;
using Stories.Parser.Statements;

namespace Stories.Parser.Parsers
{
    public static class HistoryParser
    {
        static readonly Parser<Statement> Statement =
            ValueStatementParser.After
            .Or<Statement>(ValueStatementParser.Initial)
            .Or(EffectStatementParser.Causes)
            .Or(EffectStatementParser.Impossible)
            .Or(ReleaseStatementParser.Releases)
            .Or(ConstraintStatementParser.Always)
            .Or(NonInertialStatementParser.NonInertial)
            .Token();

        public static readonly Parser<HistoryStatement> History =
            (from statements in Parse.End(Statement.Many())
             select new HistoryStatement(statements));
    }
}
