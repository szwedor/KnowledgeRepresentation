using System.Collections.Generic;
using Stories.Parser.Conditions;

namespace Stories.Parser.Statements
{
    public class ReleaseStatement : Statement
    {
        public string Action { get; set; }
        public List<string> Agents { get; set; }
        public bool IsNegated { get; set; }
        public string Fluent { get; set; }
        public ConditionExpression Condition { get; set; }
    }
}
