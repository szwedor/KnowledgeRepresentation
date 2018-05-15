using NUnit.Framework;
using Stories.Execution;
using Stories.Parser;
using Stories.Parser.Statements.QueryStatements;
using Stories.Query;

namespace Stories.Tests
{
    public class ExecutableeQueryTests
    {
        [Test]
        public void PossiblyExecutableTest()
        {
            var queryText = @"possibly executable Bilbo TakeSword, Bilbo AttackFrodo, Frodo AttackBilbo";
            var query = Parsing.GetQuery(queryText);

            var text = @"initially FrodoLives and BilboLives and not FrodoHasSword and not BilboHasSword
                        when Frodo TakeSword causes FrodoHasSword if not BilboHasSword
                        when Bilbo TakeSword causes BilboHasSword if not FrodoHasSword
                        impossible Frodo AttackFrodo
                        impossible Frodo TakeSword if not FrodoLives
                        impossible Bilbo AttackBilbo
                        impossible Bilbo TakeSword if not BilboLives
                        AttackFrodo typically causes not FrodoLives if BilboHasSword
                        AttackBilbo causes not BilboLives if FrodoHasSword
						observable not BilboLives after Frodo AttackBilbo";

            var history = Parsing.GetHistory(text);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story, null);

            if (query is ExecutableQueryStatement executableQuery)
            {
                var queryResult = executableQuery.Execute(graph, history);
                Assert.AreEqual(true, queryResult);
            }
        }

		[Test]
		public void PossiblyExecutableTest2()
		{
			var queryText = @"possibly executable Bilbo TakeSword, Frodo TakesSword";
			var query = Parsing.GetQuery(queryText);

			var text = @"initially FrodoLives and BilboLives and not FrodoHasSword and not BilboHasSword
                        when Frodo TakeSword causes FrodoHasSword if not BilboHasSword
                        when Bilbo TakeSword causes BilboHasSword if not FrodoHasSword
                        impossible Frodo AttackFrodo
                        impossible Frodo TakeSword if not FrodoLives
                        impossible Bilbo AttackBilbo
                        impossible Bilbo TakeSword if not BilboLives
                        AttackFrodo typically causes not FrodoLives if BilboHasSword
                        AttackBilbo causes not BilboLives if FrodoHasSword
						observable not BilboLives after Frodo AttackBilbo";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, null);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(false, queryResult);
			}
		}

		[Test]
		public void NecessaryExecutableTest()
		{
			var queryText = @"necessary executable Bilbo TakeSword, Bilbo AttackFrodo";
			var query = Parsing.GetQuery(queryText);

			var text = @"initially FrodoLives and BilboLives and not FrodoHasSword and not BilboHasSword
                        when Frodo TakeSword causes FrodoHasSword if not BilboHasSword
                        when Bilbo TakeSword causes BilboHasSword if not FrodoHasSword
                        impossible Frodo AttackFrodo
                        impossible Frodo TakeSword if not FrodoLives
                        impossible Bilbo AttackBilbo
                        impossible Bilbo TakeSword if not BilboLives
                        AttackFrodo typically causes not FrodoLives if BilboHasSword
                        AttackBilbo causes not BilboLives if FrodoHasSword";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, null);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(true, queryResult);
			}
		}		

		[Test]
		public void NecessaryExecutableTest2()
		{
			var queryText = @"necessary executable Frodo AttackBilbo, Bilbo AttackFrodo";
			var query = Parsing.GetQuery(queryText);

			var text = @"initially FrodoLives and BilboLives and not FrodoHasSword and not BilboHasSword
                        when Frodo TakeSword causes FrodoHasSword if not BilboHasSword
                        when Bilbo TakeSword causes BilboHasSword if not FrodoHasSword
                        impossible Frodo AttackFrodo
                        impossible Frodo TakeSword if not FrodoLives
                        impossible Bilbo AttackBilbo
                        impossible Bilbo TakeSword if not BilboLives
                        AttackFrodo typically causes not FrodoLives if BilboHasSword
                        AttackBilbo causes not BilboLives if FrodoHasSword
						observable not BilboLives after Frodo AttackBilbo";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, null);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(false, queryResult);
			}
		}

		[Test]
		public void NecessaryExecutableTest3()
		{
			var queryText = @"necessary executable Frodo TakeSword, Bilbo TakeSword";
			var query = Parsing.GetQuery(queryText);

			var text = @"initially FrodoLives and BilboLives and not FrodoHasSword and not BilboHasSword
                        when Frodo TakeSword causes FrodoHasSword if not BilboHasSword
                        when Bilbo TakeSword causes BilboHasSword if not FrodoHasSword
                        impossible Frodo AttackFrodo
                        impossible Frodo TakeSword if not FrodoLives
                        impossible Bilbo AttackBilbo
                        impossible Bilbo TakeSword if not BilboLives
                        AttackFrodo typically causes not FrodoLives if BilboHasSword
                        AttackBilbo causes not BilboLives if FrodoHasSword";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, null);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(false, queryResult);
			}
		}
	}
}
