using System.Collections.Generic;
using System.Linq;
using Sprache;
using Stories.Parser.Conditions;
using Stories.Parser.Statements;

namespace Stories.Parser.Parsers
{
    public static class CommonParser
    {
        public static readonly Parser<string> Action = Parse.Regex("[_0-9a-zA-Z]+").ExceptKeywords().Token();
        public static readonly Parser<string> Agent = Parse.Regex("[_0-9a-zA-Z]+").ExceptKeywords().Token();
        public static readonly Parser<string> Fluent = Parse.Regex("[_0-9a-zA-Z]+").ExceptKeywords().Token();

        public static readonly Parser<List<string>> Agents = Agent.DelimitedBy(Parse.String("or").Token()).Select(a => a.ToList()).Token();

        public static readonly Parser<ActionWithExecutor> ActionWithExecutor =
        (from agent in Agent
         from action in Action
         select new ActionWithExecutor
         {
             Agent = agent,
             Action = action
         }).Or(
            from action in Action
            select new ActionWithExecutor()
            {
                Action = action
            });

        public static readonly Parser<List<string>> WhenAgents =
           (from when in KeywordsParser.When
            from agents in CommonParser.Agents
            select agents.ToList()).Token();

        public static readonly Parser<ConditionExpression> IfCondition =
            (from If in KeywordsParser.If
             from condition in ConditionsParsing.Condition
             select condition).Token();

        public static readonly Parser<ConditionExpression> FromCondition =
            (from fromKeyword in KeywordsParser.From
             from conditionFrom in ConditionsParsing.Condition
             select conditionFrom).Token();

    }
}
