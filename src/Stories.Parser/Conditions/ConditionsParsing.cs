using System.Linq;
using Sprache;

namespace Stories.Parser.Conditions
{
    public static class ConditionsParsing
    {
        private static readonly Parser<OperationType> And = Parse.String("and").Token().Return(OperationType.And);
        private static readonly Parser<OperationType> Or = Parse.String("or").Token().Return(OperationType.Or);
        private static readonly Parser<OperationType> Not = Parse.String("not").Token().Return(OperationType.Not);
        private static readonly Parser<OperationType> Iff = Parse.String("iff").Token().Return(OperationType.Iff);
        private static readonly Parser<OperationType> Consequence = Parse.String("then").Token().Return(OperationType.Consequence);

        private static readonly Parser<ConditionExpression> Constant = (Parse.String("true").Return(new ConditionConstant(true)).Or(Parse.String("false").Return(new ConditionConstant(false))));
        private static readonly Parser<ConditionExpression> Variable = Parse
            .Identifier(Parse.Letter, Parse.LetterOrDigit).Token().Select(t => new ConditionVariable(t));

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
             .XOr(Constant)
             .XOr(Variable);

        private static readonly Parser<ConditionExpression> Operand = 
            ((from sign in Not
              from factor in Factor
              select new ConditionNegation(factor)
             ).XOr(Factor)).Token();

        private static readonly Parser<ConditionExpression> Condition3 = Parse.ChainOperator(And.Or(Or), Operand, MakeOperation);
        private static readonly Parser<ConditionExpression> Condition2 = Parse.ChainOperator(Consequence, Condition3, MakeOperation);
        public static readonly Parser<ConditionExpression> Condition = Parse.ChainOperator(Iff, Condition2, MakeOperation);
    }
}
