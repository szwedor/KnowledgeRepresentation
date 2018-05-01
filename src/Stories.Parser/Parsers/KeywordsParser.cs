using Sprache;

namespace Stories.Parser.Parsers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    static class KeywordsParser
    {
        private static Parser<string> Keyword(string keyword)
        {
            return Parse.String(keyword).Text().Token();
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
        public static readonly Parser<string> Then = Keyword("then");
        public static readonly Parser<string> True = Keyword("true");
        public static readonly Parser<string> False = Keyword("false");


        public static Parser<string> ExceptKeywords(this Parser<string> parser)
        {
            return parser.Except(typeof(KeywordsParser)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(p => (Parser<string>)p.GetValue(null)).Aggregate((a, b) => b = b.Or(a)));
        }

    }
}
