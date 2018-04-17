using Stories.Parser.Conditions;

namespace Stories.Parser.Statements
{
    public class AlwaysStatement : Statement
    {
        public ConditionExpression Condition { get; set; }
    }
}
