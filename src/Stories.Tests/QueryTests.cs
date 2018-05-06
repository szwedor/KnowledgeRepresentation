using NUnit.Framework;
using Stories.Execution;
using Stories.Graph;
using Stories.Graph.Model;
using Stories.Parser;
using Stories.Parser.Conditions;
using Stories.Parser.Statements.QueryStatements;
using Stories.Query;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var graph = Graph.Graph.CreateGraph(story);

            if (query is AccessibleQueryStatement accessibleQuery)
            {
                var queryResult = accessibleQuery.Execute(graph);
                Assert.AreEqual(false, queryResult);
            }      
        }
    }
}
