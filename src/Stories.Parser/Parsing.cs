using System;
using System.Collections.Generic;
using System.Linq;
using Sprache;
using Stories.Parser.Conditions;
using Stories.Parser.Statements;

namespace Stories.Parser
{
    public class History
    {
        public List<EffectStatement> Effects { get; set; }
        public List<AlwaysStatement> Always { get; set; }
        public List<NonInertialStatement> NonInertial { get; set; }
        public List<ReleaseStatement> Releases { get; set; }
        public List<ValueStatement> Values { get; set; }

        public History(IEnumerable<Statement> statements)
        {
            Effects = statements.OfType<EffectStatement>().ToList();
            Always = statements.OfType<AlwaysStatement>().ToList();
            NonInertial = statements.OfType<NonInertialStatement>().ToList();
            Releases = statements.OfType<ReleaseStatement>().ToList();
            Values = statements.OfType<ValueStatement>().ToList();
        }
    }

    public static class Parsing
    {
        private static class Keywords
        {
            private static Parser<string> Keyword(string keyword)
            {
                return Parse.String(keyword).Text().Token();
            }

            public static readonly Parser<string> Initially = Keyword("initially");
            public static readonly Parser<string> Always = Keyword("always");
            public static readonly Parser<string> Observable = Keyword("observable");
            public static readonly Parser<string> After = Keyword("after");
            public static readonly Parser<string> When = Keyword("when");
            public static readonly Parser<string> Causes = Keyword("causes");
            public static readonly Parser<string> Impossible = Keyword("impossible");
            public static readonly Parser<string> Typically = Keyword("typically");
            public static readonly Parser<string> If = Keyword("if");
        }

        public static readonly Parser<string> Action = Parse.Identifier(Parse.Letter, Parse.LetterOrDigit).Token();
        public static readonly Parser<string> Agent = Parse.Identifier(Parse.Letter, Parse.LetterOrDigit).Token();

        public static readonly Parser<ValueStatement> After =
            (from observable in Keywords.Observable.Optional().Select(v => !v.IsEmpty)
             from condition in ConditionsParsing.Condition
             from after in Keywords.After
             from actions in Parse.DelimitedBy(Action, Parse.String(","))
             select new ValueStatement
             {
                 IsObservable = observable,
                 Condition = condition,
                 Actions = actions.ToList()
             }).Token();

        public static readonly Parser<ValueStatement> Initial =
            (from initially in Keywords.Initially
             from condition in ConditionsParsing.Condition
             select new ValueStatement
             {
                 IsObservable = false,
                 Condition = condition,
                 Actions = new List<string>()
             }).Token();

        private static readonly Parser<string> WhenAgent =
            (from when in Keywords.When
             from agent in Agent
             select agent).Token();

        private static readonly Parser<ConditionExpression> IfCondition =
            (from If in Keywords.If
             from condition in ConditionsParsing.Condition
             select condition).Token();

        public static readonly Parser<EffectStatement> Causes =
            (from agent in WhenAgent.Optional()
             from action in Action
             from typically in Keywords.Typically.Optional()
             from causes in Keywords.Causes
             from effect in ConditionsParsing.Condition
             from condition in IfCondition.Optional()
             select new EffectStatement
             {
                 Agent = agent.GetOrElse(null),
                 Action = action,
                 IsTypical = !typically.IsEmpty,
                 Effect = effect,
                 Condition = condition.GetOrElse(new ConditionConstant(true))
             }).Token();

        // public static readonly Parser<EffectStatement> Impossible =
        //     (from impossible in Keywords.Impossible
        //      from action in Action
        //      from condition in IfCondition.Optional()
        //      select new EffectStatement
        //      {
        //          Agent = null,
        //          Action = action,
        //          IsTypical = false,
        //          Effect = effect,
        //          Condition = condition.GetOrElse(new ConditionConstant(true))
        //      }).Token();

        public static readonly Parser<AlwaysStatement> Always =
            (from always in Keywords.Always
             from condition in ConditionsParsing.Condition
             select new AlwaysStatement
             {
                Condition = condition   
             }).Token();

        public static readonly Parser<Statement> Statement =
            After.Or<Statement>(Initial).Or(Causes).Or(Always).Token();
        public static readonly Parser<History> History =
            (from statements in Parse.End(Statement.Many())
             select new History(statements));
    }
}
