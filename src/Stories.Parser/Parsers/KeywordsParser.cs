using Sprache;

namespace Stories.Parser.Parsers
{
    using Stories.Parser.Statements;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class KeywordsParser
    {
        private static Parser<string> Keyword(string keyword)
        {
            return Parse.String(keyword).Text()
                .Then(p=>Parse.WhiteSpace.Once()).Text().Token()
                .Or(Parse.String(keyword).Text().End());
        }


        public static readonly Parser<string> Initially = Keyword("initially");
        public static readonly Parser<string> Always = Keyword("always");
        public static readonly Parser<string> Observable = Keyword("observable");
        public static readonly Parser<string> Releases = Keyword("releases");
        public static readonly Parser<string> Noninertial = Keyword("noninertial");
        public static readonly Parser<string> After = Keyword("after");
        public static readonly Parser<string> When = Keyword("when");
        public static readonly Parser<string> Causes = Keyword("causes");
        public static readonly Parser<string> Impossible = Keyword("impossible");
        public static readonly Parser<string> Typically = Keyword("typically");
        public static readonly Parser<string> If = Keyword("if");
        public static readonly Parser<string> Iff = Keyword("iff");
        public static readonly Parser<string> And = Keyword("and");
        public static readonly Parser<string> Or = Keyword("or");
        public static readonly Parser<string> Not = Keyword("not");
        public static readonly Parser<string> Then = Keyword("then");
        public static readonly Parser<string> True = Keyword("true");
        public static readonly Parser<string> False = Keyword("false");
        public static readonly Parser<string> Executable = Keyword("executable");
        public static readonly Parser<string> From = Keyword("from");
        public static readonly Parser<string> Accessible = Keyword("accessible");
        public static readonly Parser<string> In = Keyword("in");
        public static readonly Parser<string> Necesssary = Keyword("necessary");
        public static readonly Parser<string> Possibly = Keyword("possibly");



        public static Parser<string> ExceptKeywords(this Parser<string> parser)
        {
           var parsers = typeof(KeywordsParser)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(p => (Parser<string>) p.GetValue(null)).ToList();

            return parser.Except(parsers.Aggregate(parsers.First(),(a, b) => b = b.Or(a)));
        }

    }
}
