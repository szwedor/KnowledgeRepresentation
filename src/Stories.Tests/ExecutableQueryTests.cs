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
            var graph = Graph.Graph.CreateGraph(story, query);

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
			var graph = Graph.Graph.CreateGraph(story, query);

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
			var graph = Graph.Graph.CreateGraph(story, query);

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
						impossible AttackFrodo if not BilboLives
						impossible AttackBilbo if not FrodoLives
                        AttackFrodo typically causes not FrodoLives if BilboHasSword
                        AttackBilbo causes not BilboLives if FrodoHasSword
						observable not BilboLives after Frodo AttackBilbo";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, query);

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
			var graph = Graph.Graph.CreateGraph(story, query);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(true, queryResult);
			}
		}

		[Test]
		public void NecessaryExecutableTest4()
		{
			var queryText = @"necessary executable Bob load, Bob shoot, Bob shoot";
			var query = Parsing.GetQuery(queryText);

			var text = @"initially alive and not loaded
						 load causes loaded
						 impossible shoot if not loaded
					     shoot typically causes not loaded and not alive";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, query);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(false, queryResult);
			}
		}

		[Test]
		public void PossibllyExecutableTest3()
  		{
			var queryText = @"possibly executable Bob load, Bob shoot, Bob shoot";
			var query = Parsing.GetQuery(queryText);

			var text = @"initially alive and not loaded
						 load causes loaded
						 impossible shoot if not loaded
					     shoot typically causes not loaded and not alive";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, query);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(true, queryResult);
			}
		}

		[Test]
		public void NecessaryExecutableTest5()
		{
			var queryText = @"necessary executable load, shoot, shoot";
			var query = Parsing.GetQuery(queryText);

			var text = @"initially alive and not loaded
						 load causes loaded
						 impossible shoot if not loaded
					     shoot typically causes not loaded and not alive";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, query);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(false, queryResult);
			}
		}

		[Test]
		public void PossibllyExecutableTest4()
		{
			var queryText = @"possibly executable load, shoot, shoot";
			var query = Parsing.GetQuery(queryText);

			var text = @"initially alive and not loaded
						 load causes loaded
						 impossible shoot if not loaded
					     shoot typically causes not loaded and not alive";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, query);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(true, queryResult);
			}
		}

		[Test]
		public void NecessaryExecutableTest6()
		{
			var queryText = @"necessary executable John load, John shoot, shoot";
			var query = Parsing.GetQuery(queryText);

			var text = @"initially alive and not loaded
						 load causes loaded
						 impossible shoot if not loaded
						 impossible Bob shoot
					     shoot typically causes not loaded and not alive";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, query);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(false, queryResult);
			}
		}

		[Test]
		public void PossibllyExecutableTest5()
		{
			var queryText = @"possibly executable John load, John shoot, shoot";
			var query = Parsing.GetQuery(queryText);

			var text = @"initially alive and not loaded
						 load causes loaded
						 impossible shoot if not loaded
						 impossible Bob shoot
					     shoot typically causes not loaded and not alive";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, query);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(true, queryResult);
			}
		}

		[Test]
		public void NecessaryExecutableTest7()
		{
			var queryText = @"necessary executable load, shoot, shoot";
			var query = Parsing.GetQuery(queryText);

			var text = @"initially alive and not loaded
						 load causes loaded
						 impossible shoot if not loaded
						 impossible Bob shoot
					     shoot causes not alive";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, query);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(true, queryResult);
			}
		}

		[Test]
		public void PossibllyExecutableTest6()
		{
			var queryText = @"possibly executable load, shoot, shoot";
			var query = Parsing.GetQuery(queryText);

			var text = @"initially alive and not loaded
						 load causes loaded
						 impossible shoot if not loaded
						 impossible Bob shoot
					     shoot causes not alive";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, query);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(true, queryResult);
			}
		}

		[Test]
		public void NecessaryExecutableTest8()
		{
			var queryText = @"necessary executable load, shoot, shoot";
			var query = Parsing.GetQuery(queryText);

			var text = @"initially alive and not loaded
						 load causes loaded
						 impossible shoot if not loaded
					     when Bob shoot causes not alive and not loaded
						 when John shoot causes not alive";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, query);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(true, queryResult);
			}
		}

		[Test]
		public void PossibllyExecutableTest7()
		{
			var queryText = @"possibly executable load, shoot, shoot";
			var query = Parsing.GetQuery(queryText);

			var text = @"initially alive and not loaded
						 load causes loaded
						 impossible shoot if not loaded
					     when Bob shoot causes not alive and not loaded
						 when John shoot causes not alive";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, query);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(true, queryResult);
			}
		}

		[Test]
		public void NecessaryExecutableTest9()
		{
			var queryText = @"necessary executable John load, John shoot, shoot";
			var query = Parsing.GetQuery(queryText);

			var text = @"initially alive and not loaded
						 load causes loaded
						 impossible shoot if not loaded
						 impossible Bob shoot
					     observable not loaded and not alive after shoot";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, query);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(false, queryResult);
			}
		}

		[Test]
		public void PossibllyExecutableTest8()
		{
			var queryText = @"possibly executable John load, John shoot, shoot";
			var query = Parsing.GetQuery(queryText);

			var text = @"initially alive and not loaded
						 load causes loaded
						 impossible shoot if not loaded
						 impossible Bob shoot
					     observable not loaded and not alive after shoot";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, query);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(true, queryResult);
			}
		}

		[Test]
		public void NecessaryExecutableTest10()
		{
			var queryText = @"necessary executable John load, shoot, John shoot";
			var query = Parsing.GetQuery(queryText);

			var text = @"initially alive and not loaded
						 load causes loaded
						 impossible shoot if not loaded
						 impossible Bob shoot
					     observable not loaded and not alive after John shoot";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, query);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(true, queryResult);
			}
		}

		[Test]
		public void PossibllyExecutableTest9()
		{
			var queryText = @"possibly executable John load, shoot, John shoot";
			var query = Parsing.GetQuery(queryText);

			var text = @"initially alive and not loaded
						 load causes loaded
						 impossible shoot if not loaded
						 impossible Bob shoot
					     observable not loaded and not alive after John shoot";

			var history = Parsing.GetHistory(text);
			var story = new Story(history);
			var graph = Graph.Graph.CreateGraph(story, query);

			if (query is ExecutableQueryStatement executableQuery)
			{
				var queryResult = executableQuery.Execute(graph, history);
				Assert.AreEqual(true, queryResult);
			}
		}
	}
}
