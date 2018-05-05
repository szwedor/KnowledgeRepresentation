using Stories.Parser.Statements;
using Sprache;
using Stories.Parser.Parsers.QueryParsers;

namespace Stories.Parser.Parsers
{
    public static class QueryParser
    {
        public static readonly Parser<QueryStatement> Query =
            ExecutableQueryParser.ExecutableQuery
            .Or<QueryStatement>(AfterQueryParser.AfterQuery)
            .Or(AccessibleQueryParser.AccessibleQuery)
            .Or(AgentInQueryParser.AgentInQuery)
            .Token();
    }
}
