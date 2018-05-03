using System.Linq;
using Sprache;

namespace Stories.Parser.Conditions
{
    using System.Security.Cryptography;
    using Stories.Parser.Parsers;

    public static class ConditionsParsing
    {
        private static readonly Parser<OperationType> And = KeywordsParser.And.Return(OperationType.And);
        private static readonly Parser<OperationType> Or = KeywordsParser.Or.Return(OperationType.Or);
        private static readonly Parser<OperationType> Not = KeywordsParser.Not.Return(OperationType.Not);
        private static readonly Parser<OperationType> Iff = KeywordsParser.Iff.Return(OperationType.Iff);
        private static readonly Parser<OperationType> Consequence = KeywordsParser.Then.Return(OperationType.Consequence);

        private static readonly Parser<ConditionExpression> Constant = 
            (KeywordsParser.True.Return(new ConditionConstant(true))
            .Or(KeywordsParser.False.Return(new ConditionConstant(false)))).Named("bool");
        private static readonly Parser<ConditionExpression> Variable = Parse
            .Identifier(Parse.Letter, Parse.LetterOrDigit).Token().ExceptKeywords().Select(t => new ConditionVariable(t));

        private static ConditionOperation MakeOperation(OperationType operation, ConditionExpression left, ConditionExpression right)
        {
            return new ConditionOperation()
            {
                Left = left,
                Operation = operation,
                Right = right
            };
        }

        private static readonly Parser<ConditionExpression> Factor =
            (from lparen in Parse.Char('(')
             from expr in Parse.Ref(() => Condition)
             from rparen in Parse.Char(')')
             select expr).Named("expression")
             .XOr(Variable)
             .XOr(Constant);

        private static readonly Parser<ConditionExpression> Operand = 
            ((from sign in Not.Many()
              from factor in Factor
              select sign.Count()%2==1?
                  new ConditionNegation(factor):
                  factor
             ).XOr(Factor)).Token();

        private static readonly Parser<ConditionExpression> Condition3 = Parse.ChainOperator(And.Or(Or), Operand, MakeOperation);
        private static readonly Parser<ConditionExpression> Condition2 = Parse.ChainOperator(Consequence, Condition3, MakeOperation);
        public static readonly Parser<ConditionExpression> Condition = Parse.ChainOperator(Iff, Condition2, MakeOperation);
    }
}
