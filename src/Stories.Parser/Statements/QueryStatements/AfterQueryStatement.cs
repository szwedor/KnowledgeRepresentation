using Stories.Parser.Conditions;
using System.Collections.Generic;

namespace Stories.Parser.Statements.QueryStatements
{
    public class AfterQueryStatement : QueryStatement
    {
        public List<ActionWithExecutor> Actions { get; set; }
        public ConditionExpression StateToCondition { get; set; }
        public ConditionExpression StateFromCondition { get; set; }
    }
}
