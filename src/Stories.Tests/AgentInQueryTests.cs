using NUnit.Framework;
using Stories.Execution;
using Stories.Parser;
using Stories.Parser.Statements.QueryStatements;
using Stories.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stories.Tests
{

    [TestFixtureSource(typeof(AgentInData), "Data")]
    public class AgentInQueryTests
    {
        private string query;
        private string text;
        private bool answer;
        public AgentInQueryTests(string query, string text, bool answer)
        {
            this.query = query;
            this.text = text;
            this.answer = answer;
        }
        [Test]
        public void AgentIsInQueryTest()
        {
            var queryText = this.query;
            var query = Parsing.GetQuery(queryText);

            var text = this.text;

            var history = Parsing.GetHistory(text);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story, query);

            if (query is AgentInQueryStatement accessibleQuery)
            {
                var queryResult = accessibleQuery.Execute(graph, story, history);
                Assert.AreEqual(this.answer, queryResult);
            }
        }
    }
}
