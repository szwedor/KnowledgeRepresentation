using Sprache;
using Stories.Parser.Parsers;
using Stories.Parser.Statements;
using System;

namespace Stories.Parser
{
    public static class Parsing
    {
        public static HistoryStatement GetHistory(string history)
        {
            try { return HistoryParser.History.Parse(history); }
            catch (Exception ex)
            {
                throw new Exception("Parsing history error");
            }
        }

        public static QueryStatement GetQuery(string query)
        {
            try
            {
                return QueryParser.Query.Parse(query);
            }
            catch (Exception)
            {
                throw new Exception("Parsing query error");
            }
        }
    }
}
