using System;
using System.Collections.Generic;
using Stories.Parser.Conditions;

namespace Stories.Parser.Statements
{
    public class ActionWithExecutor : ICloneable
    {
        public string Action { get; set; }
        public string Agent { get; set; }

		public object Clone()
		{
			return new ActionWithExecutor()
			{
				Action = this.Action,
				Agent = this.Agent
			};
		}

		public override bool Equals(object obj)
		{
			return Action == ((ActionWithExecutor)obj).Action && Agent == ((ActionWithExecutor)obj).Agent;
		}
	}
    
    public class ValueStatement : Statement
    {
        public bool IsObservable { get; set; }
        public ConditionExpression Condition { get; set; }
        public List<ActionWithExecutor> Actions { get; set; }
    }
}
