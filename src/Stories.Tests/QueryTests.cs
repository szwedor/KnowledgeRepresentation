using NUnit.Framework;
using Stories.Execution;
using Stories.Graph;
using Stories.Graph.Model;
using Stories.Parser;
using Stories.Parser.Conditions;
using Stories.Parser.Statements.QueryStatements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stories.Tests
{
    public class QueryTests
    {
        [Test]
        public void NecessaryAccesibleTest()
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

            var accesibleQuery = query as AccessibleQueryStatement;
            var availableVertices = graph.FindVerticesSatisfyingCondition(accesibleQuery.StateFromCondition).ToList();
            var availableDestinationVertices = graph.FindVerticesSatisfyingCondition(accesibleQuery.StateToCondition).ToList();
            var verticesToCheck = availableVertices;

            var queryResult = true;
            var closedVertices = new List<Vertex>();
            if (accesibleQuery.Sufficiency == Parser.Statements.Sufficiency.Necessary)
            {
                do
                {
                    if (!verticesToCheck.All(v => availableDestinationVertices.Contains(v)))
                    {
                        queryResult = false;
                        break;
                    }
                    closedVertices.AddRange(verticesToCheck);
                    verticesToCheck = verticesToCheck.SelectMany(x => x.EdgesOutgoing.Select(y => y.To)).ToList();
                } while (!verticesToCheck.All(v => closedVertices.Contains(v)));
            }

            Assert.AreEqual(false, queryResult);
        }
    }
}
