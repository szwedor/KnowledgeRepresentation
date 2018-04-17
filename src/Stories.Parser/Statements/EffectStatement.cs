using System.Collections.Generic;
using Stories.Parser.Conditions;

namespace Stories.Parser.Statements
{
    public class EffectStatement : Statement
    {
        public ConditionExpression Effect { get; set; }
        public ConditionExpression Condition { get; set; }
        public string Action { get; set; }
        public List<string> Agents { get; set; }
        public bool IsTypical { get; set; }
    }
}
