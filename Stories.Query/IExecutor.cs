using Stories.Graph;
using Stories.Parser.Statements;

namespace Stories.Query
{
    public abstract class Executor<T> : IExecutor<T>, IExecutor where T :QueryStatement
    {
        public bool Execute(QueryStatement query, Graph.Graph graph, HistoryStatement history)
        => Execute(query as T, graph, history);

        public abstract bool Execute(T query, Graph.Graph graph, HistoryStatement history);
    }
    public interface IExecutor<T> where T :QueryStatement
    {
        bool Execute(T query, Stories.Graph.Graph graph, HistoryStatement history);
    }
    public interface IExecutor
    {
        bool Execute(QueryStatement query, Stories.Graph.Graph graph, HistoryStatement history);
    }
}