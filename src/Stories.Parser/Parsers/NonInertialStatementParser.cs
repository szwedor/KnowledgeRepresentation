using System.Linq;
using Sprache;
using Stories.Parser.Statements;

namespace Stories.Parser.Parsers
{
    static class NonInertialStatementParser
    {
        public static readonly Parser<NonInertialStatement> NonInertial =
            (from noninertian in KeywordsParser.Noninertial
             from fluent in CommonParser.Fluent
             select new NonInertialStatement
             {
                 Fluent = fluent
             }).Token();
    }
}
