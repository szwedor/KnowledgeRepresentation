using System.Collections.Generic;

namespace Stories.Parser.Statements.QueryStatements
{
    public class ExecutableQueryStatement : QueryStatement
    {
        public List<ActionWithExecutor> Actions { get; set; }
    }
}
