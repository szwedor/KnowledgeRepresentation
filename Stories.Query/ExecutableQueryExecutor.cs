using Stories.Execution;
using Stories.Graph;
using Stories.Graph.Model;
using Stories.Parser.Conditions;
using Stories.Parser.Statements;
using Stories.Parser.Statements.QueryStatements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stories.Query
{
	public class ExecutableQueryExecutor:Executor<ExecutableQueryStatement>
	{
		private static List<ActionWithExecutor> actions;
		private static HistoryStatement history;
		private static Graph.Graph graph;

		public override bool Execute(ExecutableQueryStatement query, Graph.Graph graph, HistoryStatement history)
		{
			if (graph == null)
			{
				throw new ArgumentNullException(nameof(graph));
			}

			ExecutableQueryExecutor.actions = query.Actions;
			ExecutableQueryExecutor.history = history;
			ExecutableQueryExecutor.graph = graph;

			switch (query.Sufficiency)
			{
				case Sufficiency.Necessary:
					return GetInitialStates().Select(x => ExecuteNecessarySufficiency(0, x)).All(x => x == true);
				case Sufficiency.Possibly:
					return GetInitialStates().Select(x => ExecutePossibleSufficiency(0, x)).All(x => x == true);
				default:
					throw new InvalidOperationException();
			}
			return false;
		}

		private static bool ExecutePossibleSufficiency(int currentActionIndex, Vertex vertex)
		{
			if(currentActionIndex == actions.Count())
			{
				return true;
			}

			if(vertex == null)
			{
				return false;
			}			

			if (GetVerticesWithAfter(currentActionIndex, actions, vertex).Any(x => ExecutePossibleSufficiency(currentActionIndex + 1, x)))
			{
				return true;
			}

			return false;
		}

		private static bool ExecuteNecessarySufficiency(int currentActionIndex, Vertex vertex, List<ActionWithExecutor> actionsWithExecutors = null)
		{
			if (currentActionIndex == actions.Count())
			{
				return true;
			}

			if (vertex == null)
			{
				return true;
			}

			if(actionsWithExecutors == null)
			{
				actionsWithExecutors = ExecutableQueryExecutor.actions.Select(x => (ActionWithExecutor)x.Clone()).ToList();
			}

			if (actionsWithExecutors[currentActionIndex].Agent != null)
			{
				var nextVerticesWithAfter = GetVerticesWithAfter(currentActionIndex, actionsWithExecutors, vertex);
				if (nextVerticesWithAfter.Count() == 0)
				{
					return false;
				}

				if (nextVerticesWithAfter.Any(x => !ExecuteNecessarySufficiency(currentActionIndex + 1, x, actionsWithExecutors)))
				{
					return false;
				}
			}
			else
			{
				bool anyActorSatisfies = false;
				foreach(var a in graph.Edges.Select(x => x.Actor).Distinct())
				{
					List<ActionWithExecutor> actionsCopy = actionsWithExecutors.Select(x => (ActionWithExecutor)x.Clone()).ToList();
					actionsCopy[currentActionIndex].Agent = a;
					var nextVerticesWithAfter = GetVerticesWithAfter(currentActionIndex, actionsCopy, vertex);
					if (nextVerticesWithAfter.Count() == 0)
					{
						continue;
					}

					if(nextVerticesWithAfter.All(x => ExecuteNecessarySufficiency(currentActionIndex + 1, x, actionsCopy)))
					{
						anyActorSatisfies = true;
						break;
					}
				}	
				
				if(!anyActorSatisfies)
				{
					return false;				
				}
			}

			return true;
		}

		private static List<Vertex> GetInitialStates()
		{
			var initialVertices = graph.Vertexes;

			foreach (var val in history.Values.Where(x => x.Actions.Count == 0))
			{
				initialVertices = initialVertices.FindVerticesSatisfyingCondition(val.Condition).ToList();
			}

			return initialVertices;
		}

		private static Vertex ApplyValueStatement(Vertex currentVertex, ValueStatement valueStatement)
		{
			var vertexState = currentVertex.State.Values.ToDictionary(x => x.Key, x => x.Value);
            foreach (var fluent in valueStatement.Condition.ExtractFluentsValues())
            {
                vertexState[fluent.Key] = fluent.Value;
            }
            var vertexEvaluatingValueStatement = graph.Vertexes.Where(v => v.State.Values.All(x => vertexState[x.Key] == x.Value)).ToList();
            if (vertexEvaluatingValueStatement.Count > 1)
            {
                throw new InvalidOperationException("vertexEvaluatingValueStatement length is grater than 1.");
            }

			return vertexEvaluatingValueStatement.FirstOrDefault();
		}

		private static IEnumerable<Vertex> GetVerticesWithAfter(int currentActionIndex, List<ActionWithExecutor> currentActions, Vertex vertex)
		{
			var valueStatements = GetMatchingValueStatements(currentActionIndex, currentActions);
			var nextVertices = GetNextVertices(vertex, currentActionIndex, currentActions);
			var nextVerticesAfter = valueStatements.Any(x => x.IsObservable == false) ?
				nextVertices.SelectMany(v => valueStatements.Where(x => x.IsObservable == false).Select(x => ApplyValueStatement(v, x))) : nextVertices;

			if (valueStatements.Any(x => x.IsObservable == true))
			{
				nextVerticesAfter = nextVerticesAfter.Union(nextVertices.SelectMany(v => valueStatements.Where(x => x.IsObservable == true).Select(x => ApplyValueStatement(v, x))));
			}

			return nextVerticesAfter;
		}

		private static IEnumerable<Vertex> GetNextVertices(Vertex vertex, int currentActionIndex, List<ActionWithExecutor> actionsWithExecutors = null)
		{
			actionsWithExecutors = actionsWithExecutors ?? actions;
			return vertex.EdgesOutgoing.Where(x => (x.Action == actionsWithExecutors[currentActionIndex].Action) && (actionsWithExecutors[currentActionIndex].Agent == null || x.Actor == actionsWithExecutors[currentActionIndex].Agent)).Select(x => x.To);
		}

		private static List<ValueStatement> GetMatchingValueStatements(int currentActionIndex, List<ActionWithExecutor> actionsWithUsedExecutors)
		{
			List<ValueStatement> valueStatements = new List<ValueStatement>();
			valueStatements.AddRange(history.Values.Where(x => x.Actions.Count() > 0 && (x.Actions.SequenceEqual(actionsWithUsedExecutors.Take(currentActionIndex + 1), new ActionWithExecutorEqualityComparer()))));
			return valueStatements;
		}

		internal class ActionWithExecutorEqualityComparer : IEqualityComparer<ActionWithExecutor>
		{
			public bool Equals(ActionWithExecutor x, ActionWithExecutor y)
			{
				return x.Action == y.Action && (x.Agent == y.Agent || x.Agent == null);
			}

			public int GetHashCode(ActionWithExecutor obj)
			{
				return obj.GetHashCode();
			}
		}
	}
}
