using System.Linq;
using Sprache;
using Stories.Parser.Statements.QueryStatements;

namespace Stories.Parser.Parsers.QueryParsers
{
    public static class ExecutableQueryParser
    {
        public static readonly Parser<ExecutableQueryStatement> ExecutableQuery =
            (from sufficiency in SufficiencyParser.Sufficiency.Where(x => x != Statements.Sufficiency.Typically)
             from executable in KeywordsParser.Executable
             from actions in Parse.DelimitedBy(CommonParser.ActionWithExecutor, Parse.String(","))
             select new ExecutableQueryStatement
             {
                 Sufficiency = sufficiency,
                 Actions = actions.ToList()
             }
            ).Token();
    }
}
