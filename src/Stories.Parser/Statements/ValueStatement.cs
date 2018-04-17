using System.Collections.Generic;
using Stories.Parser.Conditions;

namespace Stories.Parser.Statements
{
    public class ActionWithExecutor
    {
        public string Action { get; set; }
        public string Agent { get; set; }
    }
    
    public class ValueStatement : Statement
    {
        public bool IsObservable { get; set; }
        public ConditionExpression Condition { get; set; }
        public List<ActionWithExecutor> Actions { get; set; }
    }
}
