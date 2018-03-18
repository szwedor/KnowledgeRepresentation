using System.Collections.Generic;
using Stories.Parser.Conditions;

namespace Stories.Parser.Statements
{
    public class ValueStatement : Statement
    {
        public bool IsObservable { get; set; }
        public ConditionExpression Condition { get; set; }
        public List<string> Actions { get; set; }
    }
}
