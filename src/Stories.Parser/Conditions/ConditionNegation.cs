namespace Stories.Parser.Conditions
{
    public class ConditionNegation : ConditionExpression
    {
        public ConditionExpression Expression { get; }

        public ConditionNegation(ConditionExpression expression)
        {
            Expression = expression;
        }
    }
}
