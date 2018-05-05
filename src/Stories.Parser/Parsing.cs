using Sprache;
using Stories.Parser.Parsers;
using Stories.Parser.Statements;

namespace Stories.Parser
{
    public static class Parsing
    {
        public static HistoryStatement GetHistory(string history)
        {
            return HistoryParser.History.Parse(history);
        }

        public static QueryStatement GetQuery(string query)
        {
            return QueryParser.Query.Parse(query);
        }
    }
}
