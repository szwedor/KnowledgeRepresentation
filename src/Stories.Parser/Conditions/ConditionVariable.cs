namespace Stories.Parser.Conditions
{
    public class ConditionVariable : ConditionExpression
    {
        public string Label { get; }

        public ConditionVariable(string label)
        {
            Label = label;
        }
    }
}
