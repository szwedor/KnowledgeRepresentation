namespace Stories.Parser.Conditions
{
    public class ConditionOperation : ConditionExpression
    {
        public ConditionExpression Left { get; set; }
        public OperationType Operation { get; set; }
        public ConditionExpression Right { get; set; }
    }
}
