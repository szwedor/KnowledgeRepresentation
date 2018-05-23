using NUnit.Framework;
using Stories.Execution;
using Stories.Parser;
using Stories.Parser.Statements.QueryStatements;
using Stories.Query;

namespace Stories.Tests
{
    public class AfterQueryTests
    {
        [Test]
        public void NecessaryAfterTest()
        {
            var queryText = @"necessary not BilboLives after Frodo TakeSword, Frodo AttackBilbo from BilboLives and FrodoLives and not FrodoHasSword";
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

            if (query is AfterQueryStatement afterQuery)
            {
                var queryResult = afterQuery.Execute(graph, history);
                Assert.AreEqual(false, queryResult);
            }
        }

        [Test]
        public void PossiblyAfterTest()
        {
            var queryText = @"possibly not BilboLives after Bilbo TakeSword, Bilbo AttackFrodo, Frodo AttackBilbo from BilboLives and FrodoLives";
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

            if (query is AfterQueryStatement afterQuery)
            {
                var queryResult = afterQuery.Execute(graph, history);
                Assert.AreEqual(true, queryResult);
            }
        }

        [Test]
        public void TypicallyAfterTest()
        {
            var queryText = @"typically not FrodoLives after Bilbo TakeSword, Bilbo AttackFrodo from BilboLives and FrodoLives and not FrodoHasSword";
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

            if (query is AfterQueryStatement afterQuery)
            {
                var queryResult = afterQuery.Execute(graph, history);
                Assert.AreEqual(true, queryResult);
            }
        }
    }
}
