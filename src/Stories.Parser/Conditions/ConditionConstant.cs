namespace Stories.Parser.Conditions
{
    public class ConditionConstant : ConditionExpression
    {
        public bool Value { get; }

        public ConditionConstant(bool value)
        {
            Value = value;
        }
    }
}
