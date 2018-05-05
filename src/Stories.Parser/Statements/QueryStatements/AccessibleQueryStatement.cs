using Stories.Parser.Conditions;
using System.Collections.Generic;

namespace Stories.Parser.Statements.QueryStatements
{
    public class AccessibleQueryStatement : QueryStatement
    {
        public ConditionExpression StateToCondition { get; set; }
        public ConditionExpression StateFromCondition { get; set; }
    }
}
