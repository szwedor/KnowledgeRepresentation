using System.Linq;
using Sprache;
using Stories.Parser.Statements.QueryStatements;

namespace Stories.Parser.Parsers.QueryParsers
{
    public static class AgentInQueryParser
    {
        public static readonly Parser<AgentInQueryStatement> AgentInQuery =
            (from sufficiency in SufficiencyParser.Sufficiency.Where(x => x != Statements.Sufficiency.Typically)
             from agent in CommonParser.Agent
             from inKeyword in KeywordsParser.In
             from actions in Parse.DelimitedBy(CommonParser.ActionWithExecutor, Parse.String(","))
             select new AgentInQueryStatement
             {
                 Sufficiency = sufficiency,
                 Actions = actions.ToList(),
                 Agent = agent
             }
            ).Token();
    }
}