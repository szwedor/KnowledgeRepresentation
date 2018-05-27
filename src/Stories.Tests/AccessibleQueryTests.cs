using NUnit.Framework;
using Stories.Execution;
using Stories.Parser;
using Stories.Parser.Statements.QueryStatements;
using Stories.Query;

namespace Stories.Tests
{
    public class AccessibleQueryTests
    {
        [Test]
        public void NecessaryAccessibleTest()
        {
            var queryText = @"necessary accessible BilboLives and FrodoLives from BilboLives and not FrodoHasSword";
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

            if (query is AccessibleQueryStatement accessibleQuery)
            {
                var queryResult = accessibleQuery.Execute(graph, history);
                Assert.AreEqual(true, queryResult);
            }
        }

        [Test]
        public void NecessaryAccessibleTestIssue27()
        {
            var queryText = @"necessary accessible open from hasCard";
            var query = Parsing.GetQuery(queryText);

            var text = @"initially not open
                        insertCard typically causes open
                        impossible insertCard if not hasCard";

            var history = Parsing.GetHistory(text);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story, null);

            if (query is AccessibleQueryStatement accessibleQuery)
            {
                var queryResult = accessibleQuery.Execute(graph, history);
                Assert.AreEqual(false, queryResult);
            }
        }

        [Test]
        public void NecessaryAccessibleTestIssue29()
        {
            var queryText = @"necessary accessible open from not open";
            var query = Parsing.GetQuery(queryText);

            var text = @"initially not open
                        hasCard after Jim spawnCard
                        insertCard causes open
                        impossible insertCard if not hasCard";

            var history = Parsing.GetHistory(text);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story, null);

            if (query is AccessibleQueryStatement accessibleQuery)
            {
                var queryResult = accessibleQuery.Execute(graph, history);
                Assert.AreEqual(true, queryResult);
            }
        }

        [Test]
        public void NecessaryAccessibleTestRelease()
        {
            var queryText = @"necessary accessible open from not open";
            var query = Parsing.GetQuery(queryText);

            var text = @"initially not open
                        hasCard after Jim spawnCard
                        insertCard releases open
                        impossible insertCard if not hasCard";

            var history = Parsing.GetHistory(text);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story, null);

            if (query is AccessibleQueryStatement accessibleQuery)
            {
                var queryResult = accessibleQuery.Execute(graph, history);
                Assert.AreEqual(false, queryResult);
            }
        }

        [Test]
        public void NecessaryAccessibleTest2()
        {
            var queryText = @"necessary accessible BilboLives from BilboLives and not FrodoHasSword";
            var query = Parsing.GetQuery(queryText);

            var text = @"initially FrodoLives and BilboLives and not FrodoHasSword and not BilboHasSword
                        when Frodo TakeSword causes FrodoHasSword if not BilboHasSword
                        when Bilbo TakeSword causes BilboHasSword if not FrodoHasSword
                        impossible Frodo AttackFrodo
                        impossible Frodo TakeSword if not FrodoLives
                        impossible Bilbo AttackBilbo
                        impossible Bilbo TakeSword if not BilboLives
                        AttackFrodo typically causes not FrodoLives if BilboHasSword";

            var history = Parsing.GetHistory(text);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story, null);

            if (query is AccessibleQueryStatement accessibleQuery)
            {
                var queryResult = accessibleQuery.Execute(graph, history);
                Assert.AreEqual(true, queryResult);
            }
        }

        [Test]
        public void NecessaryAccessibleTest3()
        {
            var queryText = @"necessary accessible BilboLives from BilboLives and BilboHasSword and not FrodoHasSword";
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

            if (query is AccessibleQueryStatement accessibleQuery)
            {
                var queryResult = accessibleQuery.Execute(graph, history);
                Assert.AreEqual(false, queryResult);
            }
        }

        [Test]
        public void TypicallyAccessibleTest()
        {
            var queryText = @"typically accessible not FrodoLives from BilboHasSword";
            var queryText2 = @"typically accessible FrodoLives from BilboHasSword";
            var query = Parsing.GetQuery(queryText);
            var query2 = Parsing.GetQuery(queryText2);

            var text = @"initially FrodoLives and BilboLives and not FrodoHasSword
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

            if (query is AccessibleQueryStatement accessibleQuery)
            {
                var queryResult = accessibleQuery.Execute(graph, history);
                Assert.AreEqual(true, queryResult);
            }
            if (query2 is AccessibleQueryStatement accessibleQuery2)
            {
                var queryResult = accessibleQuery2.Execute(graph, history);
                Assert.AreEqual(true, queryResult);
            }

        }

        [Test]
        public void NecessaryAccessibleTest5()
        {
            var queryText = @"necessary accessible not FrodoLives from BilboHasSword and FrodoLives";        
            var query = Parsing.GetQuery(queryText);

            var text = @"initially FrodoLives and BilboLives
                        when Frodo TakeSword causes FrodoHasSword
                        when Bilbo TakeSword causes BilboHasSword
                        impossible Frodo AttackFrodo
                        impossible Frodo TakeSword if not FrodoLives
                        impossible Bilbo AttackBilbo
                        impossible Bilbo TakeSword if not BilboLives
                        AttackFrodo typically causes not FrodoLives if BilboHasSword
                        AttackBilbo causes not BilboLives if FrodoHasSword";

            var history = Parsing.GetHistory(text);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story, null);

            if (query is AccessibleQueryStatement accessibleQuery)
            {
                var queryResult = accessibleQuery.Execute(graph, history);
                Assert.AreEqual(false, queryResult);
            }
        }

        [Test]
        public void TypicallyAccessibleTest2()
        {
            var queryText = @"typically accessible BilboHasSword from not BilboHasSword";
            var query = Parsing.GetQuery(queryText);

            var text = @"initially FrodoLives and BilboLives
                        when Frodo TakeSword causes FrodoHasSword
                        when Bilbo TakeSword typically causes not BilboHasSword
                        impossible Frodo AttackFrodo
                        impossible Frodo TakeSword if not FrodoLives
                        impossible Bilbo AttackBilbo
                        impossible Bilbo TakeSword if not BilboLives
                        AttackFrodo typically causes not FrodoLives if BilboHasSword
                        AttackBilbo causes not BilboLives if FrodoHasSword";

            var history = Parsing.GetHistory(text);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story, null);

            if (query is AccessibleQueryStatement accessibleQuery)
            {
                var queryResult = accessibleQuery.Execute(graph, history);
                Assert.AreEqual(false, queryResult);
            }
        }

        [Test]
        public void NecessaryAccessibleTest6()
        {
            var queryText = @"necessary accessible not FrodoLives from FrodoHasSword";
            var query = Parsing.GetQuery(queryText);

            var text = @"initially FrodoLives and BilboLives and FrodoHasSword
                        when Frodo TakeSword causes FrodoHasSword
                        when Bilbo TakeSword causes true
                        when Bilbo TakeSword causes BilboHasSword if not FrodoHasSword
                        not FrodoHasSword  after Bilbo TakeSword
                        impossible Frodo AttackFrodo
                        impossible Frodo TakeSword if not FrodoLives
                        impossible Bilbo AttackBilbo
                        impossible Bilbo TakeSword if not BilboLives
                        AttackFrodo causes not FrodoLives if BilboHasSword
                        AttackBilbo causes not BilboLives if FrodoHasSword";

            var history = Parsing.GetHistory(text);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story, null);

            if (query is AccessibleQueryStatement accessibleQuery)
            {
                var queryResult = accessibleQuery.Execute(graph, history);
                Assert.AreEqual(true, queryResult);
            }
        }

        [Test]
        public void NecessaryAccessibleTest7()
        {
            var queryText = @"necessary accessible not FrodoLives from FrodoHasSword";
            var query = Parsing.GetQuery(queryText);

            var text = @"initially FrodoLives and BilboLives and FrodoHasSword
                        when Frodo TakeSword causes FrodoHasSword
                        when Bilbo TakeSword causes BilboHasSword if not FrodoHasSword
                        not FrodoHasSword  after Bilbo TakeSword
                        impossible Frodo AttackFrodo
                        impossible Frodo TakeSword if not FrodoLives
                        impossible Bilbo AttackBilbo
                        impossible Bilbo TakeSword if not BilboLives
                        AttackFrodo typically causes not FrodoLives if BilboHasSword
                        AttackBilbo causes not BilboLives if FrodoHasSword";

            var history = Parsing.GetHistory(text);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story, null);

            if (query is AccessibleQueryStatement accessibleQuery)
            {
                var queryResult = accessibleQuery.Execute(graph, history);
                Assert.AreEqual(false, queryResult);
            }
        }

        [Test]
        public void TypicallyAccessibleTest3()
        {
            var queryText = @"typically accessible not FrodoLives from FrodoHasSword";
            var query = Parsing.GetQuery(queryText);

            var text = @"initially FrodoLives and BilboLives and FrodoHasSword
                        when Frodo TakeSword causes FrodoHasSword
                        when Bilbo TakeSword causes BilboHasSword if not FrodoHasSword
                        not FrodoHasSword  after Bilbo TakeSword
                        impossible Frodo AttackFrodo
                        impossible Frodo TakeSword if not FrodoLives
                        impossible Bilbo AttackBilbo
                        impossible Bilbo TakeSword if not BilboLives
                        AttackFrodo typically causes not FrodoLives if BilboHasSword
                        AttackBilbo causes not BilboLives if FrodoHasSword";

            var history = Parsing.GetHistory(text);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story, null);

            if (query is AccessibleQueryStatement accessibleQuery)
            {
                var queryResult = accessibleQuery.Execute(graph, history);
                Assert.AreEqual(true, queryResult);
            }
        }

        [Test]
        public void PossiblyAccessibleTest()
        {
            var queryText = @"possibly accessible not FrodoLives from FrodoHasSword";
            var query = Parsing.GetQuery(queryText);

            var text = @"initially FrodoLives and BilboLives and FrodoHasSword
                        when Frodo TakeSword causes FrodoHasSword
                        when Bilbo TakeSword causes BilboHasSword if not FrodoHasSword
                        not FrodoHasSword  after Bilbo TakeSword
                        impossible Frodo AttackFrodo
                        impossible Frodo TakeSword if not FrodoLives
                        impossible Bilbo AttackBilbo
                        impossible Bilbo TakeSword if not BilboLives
                        AttackFrodo typically causes not FrodoLives if BilboHasSword
                        AttackBilbo causes not BilboLives if FrodoHasSword";

            var history = Parsing.GetHistory(text);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story, null);

            if (query is AccessibleQueryStatement accessibleQuery)
            {
                var queryResult = accessibleQuery.Execute(graph, history);
                Assert.AreEqual(true, queryResult);
            }
        }

        [Test]
        public void PossiblyAccessibleTest1()
        {
            var queryText = @"possibly accessible FrodoLives from not FrodoLives";
            var query = Parsing.GetQuery(queryText);

            var text = @"initially FrodoLives and BilboLives and FrodoHasSword
                        when Frodo TakeSword causes FrodoHasSword
                        when Bilbo TakeSword causes BilboHasSword if not FrodoHasSword
                        not FrodoHasSword  after Bilbo TakeSword
                        impossible Frodo AttackFrodo
                        impossible Frodo TakeSword if not FrodoLives
                        impossible Bilbo AttackBilbo
                        impossible Bilbo TakeSword if not BilboLives
                        AttackFrodo typically causes not FrodoLives if BilboHasSword
                        AttackBilbo causes not BilboLives if FrodoHasSword";

            var history = Parsing.GetHistory(text);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story, null);

            if (query is AccessibleQueryStatement accessibleQuery)
            {
                var queryResult = accessibleQuery.Execute(graph, history);
                Assert.AreEqual(false, queryResult);
            }
        }
        [Test]
        public void RealseseTest1()
        {
            var queryText = @"possibly accessible not loaded from loaded";
            var query = Parsing.GetQuery(queryText);

            var text = @"initially loaded
                shoot releases loaded";

            var history = Parsing.GetHistory(text);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story, null);

            if (query is AccessibleQueryStatement accessibleQuery)
            {
                var queryResult = accessibleQuery.Execute(graph, history);
                Assert.AreEqual(true, queryResult);
            }
        }
    }
}
