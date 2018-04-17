using Sprache;

namespace Stories.Parser.Parsers
{
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
    }
}
