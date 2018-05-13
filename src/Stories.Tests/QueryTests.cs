﻿using NUnit.Framework;
using Stories.Execution;
using Stories.Parser;
using Stories.Parser.Statements.QueryStatements;
using Stories.Query;

namespace Stories.Tests
{
    public class QueryTests
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
    }
}
