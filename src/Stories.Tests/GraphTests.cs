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
            var graph = Graph.Graph.CreateGraph(story, null);

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

        [Test]
        public void TypicallyWithCertainEffectTest()
        {
            var spaghetti = @"
when Michal ZrobSpaghetti typically causes spaghetti if not spaghetti
when Michal ZrobSpaghetti causes tortilla if not spaghetti";

            var history = Parsing.GetHistory(spaghetti);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story, null);

            var q0 = story.States.First(p => !p.GetVariable("spaghetti") && !p.GetVariable("tortilla"));
            var qTypical = story.States.First(p => p.GetVariable("spaghetti") && p.GetVariable("tortilla"));
            var qNonTypical = story.States.First(p => !p.GetVariable("spaghetti") && p.GetVariable("tortilla"));
            graph.Edges.Should().Contain(p => p.From.State.Equals(q0) && p.To.State.Equals(qTypical)
            && p.Action == "ZrobSpaghetti" && p.Actor=="Michal" && p.IsTypical);
            graph.Edges.Should().Contain(p => p.From.State.Equals(q0) && p.To.State.Equals(qNonTypical)
            && p.Action == "ZrobSpaghetti" && p.Actor=="Michal"  && !p.IsTypical);
         
        }
        [Test]
        public void TypicallyWithCertainEffectWithoutActorTest()
        {
            var spaghetti = @"
when Michal ZrobSpaghetti typically causes spaghetti if not spaghetti
ZrobSpaghetti causes tortilla if not spaghetti";

            var history = Parsing.GetHistory(spaghetti);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story, null);

            var q0 = story.States.First(p => !p.GetVariable("spaghetti") && !p.GetVariable("tortilla"));
            var qTypical = story.States.First(p => p.GetVariable("spaghetti") && p.GetVariable("tortilla"));
            var qNonTypical = story.States.First(p => !p.GetVariable("spaghetti") && p.GetVariable("tortilla"));
            graph.Edges.Should().Contain(p => p.From.State.Equals(q0) && p.To.State.Equals(qTypical)
            && p.Action == "ZrobSpaghetti" && p.Actor == "Michal" && p.IsTypical);
            graph.Edges.Should().Contain(p => p.From.State.Equals(q0) && p.To.State.Equals(qNonTypical)
            && p.Action == "ZrobSpaghetti" && p.Actor == "Michal" && !p.IsTypical);

        }

        [Test]
        public void TypicallyEffectTest()
        {
            var spaghetti = @"when Michal ZrobSpaghetti typically causes spaghetti if not spaghetti";

            var history = Parsing.GetHistory(spaghetti);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story, null);

            var q0 = story.States.First(p => !p.GetVariable("spaghetti"));
            var qTypical = story.States.First(p => p.GetVariable("spaghetti"));
            var qNonTypical = q0;

            graph.Edges.Should().Contain(p => p.From.State.Equals(q0) && p.To.State.Equals(qTypical)
            && p.Action == "ZrobSpaghetti" && p.Actor == "Michal" && p.IsTypical);
            graph.Edges.Should().Contain(p => p.From.State.Equals(q0) && p.To.State.Equals(qNonTypical)
            && p.Action == "ZrobSpaghetti" && p.Actor == "Michal" && !p.IsTypical);

        }

        [Test]
        public void Test()
        {
            var spaghetti = @"initially not An and not At
initially not Bn and not Bt
initially not Ct and not Cn
initially start
always (start then not An)
always (start then not At)
always (At then not An)
always (An then not At)
always (Bt then not Bn)
always (Bn then not Bt)
always (Ct then not Cn)
always (Cn then not Ct)
impossible A if not start or An or At or Bn or Bt or Cn or Ct
impossible B if start or ( not An and not At)  or Bn or Bt or Cn or Ct
impossible C if start or ( not An and not At) or ( not Bn and not Bt) or Cn or Ct
A causes An and not start
B typically causes Bt if An
C typically causes Ct if An and Bt
C causes Cn if An and Bt

A typically causes At and not start
B causes Bn if At
C typically causes Ct if At and Bn
C causes Cn if At and Bn";

            var history = Parsing.GetHistory(spaghetti);
            var story = new Story(history);
            var graph = Graph.Graph.CreateGraph(story, null);

        }
    }
}
