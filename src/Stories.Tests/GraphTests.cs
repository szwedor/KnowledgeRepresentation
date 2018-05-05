using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Stories.Execution;
using Stories.Parser;
using Stories.Graph;

namespace Stories.Tests
{
    
    public class GraphTests
    {
        [Test]
        public void GraphYaleStoryTest()
        {
            var yaleShootingProblem = @"
            initially not loaded
            initially alive
            load causes loaded
            shoot causes not loaded
            shoot typically causes not alive if loaded";

            var history = Parsing.GetHistory(yaleShootingProblem);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story);

            var q0 = story.States.First(p => !p.GetVariable("loaded") && p.GetVariable("alive"));
            var q1 = story.States.First(p => p.GetVariable("loaded") && p.GetVariable("alive"));
            var q2 = story.States.First(p => !p.GetVariable("loaded") && !p.GetVariable("alive"));
            var q3 = story.States.First(p => p.GetVariable("loaded") && !p.GetVariable("alive"));

            graph.Vertexes.Count.Should().Be(story.States.Count);
            graph.Edges.Should().Contain(p => p.From.State.Equals(q0) && p.To.State.Equals(q1) && p.Action == "load" && p.IsTypical);
            graph.Edges.Should().Contain(p => p.From.State.Equals(q0) && p.To.State.Equals(q0) && p.Action == "shoot" && p.IsTypical);
            graph.Edges.Should().Contain(p => p.From.State.Equals(q1) && p.To.State.Equals(q0) && p.Action == "shoot" && !p.IsTypical);
            graph.Edges.Should().Contain(p => p.From.State.Equals(q1) && p.To.State.Equals(q1) && p.Action == "load" && p.IsTypical);
            graph.Edges.Should().Contain(p => p.From.State.Equals(q1) && p.To.State.Equals(q2) && p.Action == "shoot" && p.IsTypical);
            graph.Edges.Should().Contain(p => p.From.State.Equals(q2) && p.To.State.Equals(q2) && p.Action == "shoot" && p.IsTypical);
            graph.Edges.Should().Contain(p => p.From.State.Equals(q2) && p.To.State.Equals(q3) && p.Action == "load" && p.IsTypical);
            graph.Edges.Should().Contain(p => p.From.State.Equals(q3) && p.To.State.Equals(q2) && p.Action == "shoot" && p.IsTypical);
            graph.Edges.Should().Contain(p => p.From.State.Equals(q3) && p.To.State.Equals(q3) && p.Action == "load" && p.IsTypical);

        }
    }
}
