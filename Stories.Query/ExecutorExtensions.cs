using Stories.Parser.Conditions;
using Stories.Parser.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stories.Query
{
    public static class ExecutorExtensions
    {
        private static IExecutor Execute<T>(T qt) where T : QueryStatement
        {
            var type = typeof(IExecutor<>).MakeGenericType(qt.GetType());
            return (IExecutor)DI.Container.GetInstance(type);
        }
        public static bool Execute<T>(this T st,Stories.Graph.Graph graph, HistoryStatement history) 
            where T: QueryStatement
        {
            return Execute(st).Execute(st, graph, history);
        }
        public static ConditionExpression GetStartCondition(this Stories.Graph.Graph graph, HistoryStatement history)
        {
            ConditionExpression cond = new ConditionConstant(true);
            var vertices = graph.Vertexes.ToList();
            foreach (var val in history.Values.Where(x => x.Actions.Count == 0))
            {
                cond = new ConditionOperation()
                { Left = cond, Right = val.Condition, Operation = OperationType.And };
            }
           return cond;
        }
    }
}
