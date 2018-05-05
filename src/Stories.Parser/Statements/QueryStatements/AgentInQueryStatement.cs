using System.Collections.Generic;

namespace Stories.Parser.Statements.QueryStatements
{
    public class AgentInQueryStatement : QueryStatement
    {
        public string Agent { get; set; }
        public List<ActionWithExecutor> Actions { get; set; }
    }
}
