namespace Stories.Tests
{
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices.ComTypes;
    using FluentAssertions;
    using NUnit.Framework;
    using Stories.Execution;
    using Stories.Parser;

    public class StoryTests
    {
      [Test]
        public void YaleStoryTest()
        {
            var yaleShootingProblem = @"
            initially not loaded
            initially alive
            load causes loaded
            shoot causes not loaded
            shoot typically causes not alive if loaded";

            var history = Parsing.GetHistory(yaleShootingProblem);
            var story = new Story(history);

            var q0 = story.States.First(p => !p.GetVariable("loaded") && p.GetVariable("alive"));
            var q1 = story.States.First(p => p.GetVariable("loaded") && p.GetVariable("alive"));
            var q2 = story.States.First(p => !p.GetVariable("loaded") && !p.GetVariable("alive"));
            var q3 = story.States.First(p => p.GetVariable("loaded") && !p.GetVariable("alive"));


            //shoot q0
            story.Res0(null, "shoot", q0).ToArray().Should().BeEquivalentTo(q0, q2);
            story.ResMinus(null, "shoot", q0).ToArray().Should().BeEquivalentTo(q0);
            story.Res0Plus(null, "shoot", q0).ToArray().Should().BeEquivalentTo(q0, q2);
            story.ResN(null, "shoot", q0).ToArray().Should().BeEquivalentTo(q0);
            story.ResAb(null, "shoot", q0).ToArray().Should().BeEmpty();
            
            //load q0
            story.Res0(null, "load", q0).ToArray().Should().BeEquivalentTo(q1, q3);
            story.ResMinus(null, "load", q0).ToArray().Should().BeEquivalentTo(q1);
            story.Res0Plus(null, "load", q0).ToArray().Should().BeEquivalentTo(q1, q3);
            story.ResN(null, "load", q0).ToArray().Should().BeEquivalentTo(q1);
            story.ResAb(null, "load", q0).ToArray().Should().BeEmpty();
            
            //shoot q1
            story.Res0(null, "shoot", q1).ToArray().Should().BeEquivalentTo(q0, q2);
            story.ResMinus(null, "shoot", q1).ToArray().Should().BeEquivalentTo(q0);
            story.Res0Plus(null, "shoot", q1).ToArray().Should().BeEquivalentTo(q2);
            story.ResN(null, "shoot", q1).ToArray().Should().BeEquivalentTo(q2);
            story.ResAb(null, "shoot", q1).ToArray().Should().BeEquivalentTo(q0);
            //load q1
            story.Res0(null, "load", q1).ToArray().Should().BeEquivalentTo(q1, q3);
            story.ResMinus(null, "load", q1).ToArray().Should().BeEquivalentTo(q1);
            story.Res0Plus(null, "load", q1).ToArray().Should().BeEquivalentTo(q1, q3);
            story.ResN(null, "load", q1).ToArray().Should().BeEquivalentTo(q1);
            story.ResAb(null, "load", q1).ToArray().Should().BeEmpty();
            
            ////shoot q2
            story.Res0(null, "shoot", q2).ToArray().Should().BeEquivalentTo(q0, q2);
            story.ResMinus(null, "shoot", q2).ToArray().Should().BeEquivalentTo(q2);
            story.Res0Plus(null, "shoot", q2).ToArray().Should().BeEquivalentTo(q0, q2);
            story.ResN(null, "shoot", q2).ToArray().Should().BeEquivalentTo(q2);
            story.ResAb(null, "shoot", q2).ToArray().Should().BeEmpty();
            //load q2
            story.Res0(null, "load", q2).ToArray().Should().BeEquivalentTo(q1, q3);
            story.ResMinus(null, "load", q2).ToArray().Should().BeEquivalentTo(q3);
            story.Res0Plus(null, "load", q2).ToArray().Should().BeEquivalentTo(q1, q3);
            story.ResN(null, "load", q2).ToArray().Should().BeEquivalentTo(q3);
            story.ResAb(null, "load", q2).ToArray().Should().BeEmpty();

            ////shoot q3
            story.Res0(null, "shoot", q3).ToArray().Should().BeEquivalentTo(q0, q2);
            story.ResMinus(null, "shoot", q3).ToArray().Should().BeEquivalentTo(q2);
            story.Res0Plus(null, "shoot", q3).ToArray().Should().BeEquivalentTo(q2);
            story.ResN(null, "shoot", q3).ToArray().Should().BeEquivalentTo(q2);
            story.ResAb(null, "shoot", q3).ToArray().Should().BeEmpty();
            //load q3
            story.Res0(null, "load", q3).ToArray().Should().BeEquivalentTo(q1, q3);
            story.ResMinus(null, "load", q3).ToArray().Should().BeEquivalentTo(q3);
            story.Res0Plus(null, "load", q3).ToArray().Should().BeEquivalentTo(q1, q3);
            story.ResN(null, "load", q3).ToArray().Should().BeEquivalentTo(q3);
            story.ResAb(null, "load", q3).ToArray().Should().BeEmpty();
        }

        [Test]
        public void BilgoTest()
        {
            var text = @"initially FrodoLives and BilboLives and not FrodoHasSword and not BilboHasSword
                        when Frodo TakeSword causes FrodoHasSword if not BilboHasSword
                        when Bilbo TakeSword causes BilboHasSword if not FrodoHasSword
                        impossible Frodo AttackFrodo
                        impossible Frodo TakeSword if not FrodoLives
                        impossible Bilbo AttackBilbo
                        impossible Bilbo TakeSword if not BilboLives
                        AttackFrodo typically causes not FrodoLives if BilboHasSword
                        AttackBilbo causes not BilboLives if FrodoHasSword
                        observable not BilboLives after AttackBilbo ";

            var history = Parsing.GetHistory(text);
            var story = new Story(history);

            var q0 = story.States.First(
                p => p.GetVariable("FrodoLives") &&
                     p.GetVariable("BilboLives") &&
                     !p.GetVariable("FrodoHasSword") &&
                     !p.GetVariable("BilboHasSword"));
            var q1 = story.States.First(
                p => p.GetVariable("FrodoLives") &&
                     p.GetVariable("BilboLives") &&
                     p.GetVariable("FrodoHasSword") &&
                     !p.GetVariable("BilboHasSword"));
            var q2 = story.States.First(
                p => p.GetVariable("FrodoLives") &&
                     p.GetVariable("BilboLives") &&
                     !p.GetVariable("FrodoHasSword") &&
                     p.GetVariable("BilboHasSword"));
            var q3 = story.States.First(
                p => !p.GetVariable("FrodoLives") &&
                     p.GetVariable("BilboLives") &&
                     !p.GetVariable("FrodoHasSword") &&
                     p.GetVariable("BilboHasSword"));
            var q4 = story.States.First(
                p => p.GetVariable("FrodoLives") &&
                     !p.GetVariable("BilboLives") &&
                     p.GetVariable("FrodoHasSword") &&
                     !p.GetVariable("BilboHasSword"));

            var q5 = story.States.First(
                p => p.GetVariable("FrodoLives") &&
                     !p.GetVariable("BilboLives") &&
                     !p.GetVariable("FrodoHasSword") &&
                     !p.GetVariable("BilboHasSword"));

            var q6 = story.States.First(
                p => p.GetVariable("FrodoLives") &&
                     !p.GetVariable("BilboLives") &&
                     !p.GetVariable("FrodoHasSword") &&
                     p.GetVariable("BilboHasSword"));

            var g= Graph.Graph.CreateGraph(story, null);
            g.Vertexes.First(p => p.State == q0)
                .ShouldHaveEdges("Frodo", q1)//q5 nie ma bo musi miec miecz
                .ShouldHaveEdges("Bilbo", q2);

            g.Vertexes.First(p => p.State == q1)
                .ShouldHaveEdges("Frodo", "AttackBilbo", q4)
                .ShouldHaveEdges("Frodo", "TakeSword", q1);

            g.Vertexes.First(p => p.State == q2)
                .ShouldHaveEdges("Bilbo", q2, q3);
        }

        [Test]
        public void SpaghettiAndTortillaTest()
        {
            var spaghettiAndTortilla = @"
                initially not spaghetti and not tortilla
                ZrobTortille causes tortilla if not tortilla
                when Jan ZrobSpaghetti causes spaghetti if not spaghetti
                when Michał ZrobSpaghetti typically causes spaghetti if not spaghetti
                impossible Michał ZjedzSpaghetti
                ZjedzSpaghetti causes not spaghetti if spaghetti
                ZjedzTortille causes not tortilla if tortilla
                ";

            var history = Parsing.GetHistory(spaghettiAndTortilla);
            var story = new Story(history);

            var q0 = story.States.First(p => !p.GetVariable("spaghetti") && !p.GetVariable("tortilla"));
            var q1 = story.States.First(p => !p.GetVariable("spaghetti") && p.GetVariable("tortilla"));
            var q2 = story.States.First(p => p.GetVariable("spaghetti") && !p.GetVariable("tortilla"));
            var q3 = story.States.First(p => p.GetVariable("spaghetti") && p.GetVariable("tortilla"));

            var g = Graph.Graph.CreateGraph(story, null);
            //agenci
            string jan = "Jan";
            string michał = "Michał";
            //akcje
            string zjedzSpaghetti = "ZjedzSpaghetti";
            string zjedzTortille = "ZjedzTortille";
            string zrobSpaghetti = "ZrobSpaghetti";
            string zrobTortille = "ZrobTortille";

            g.Vertexes.First(p => p.State == q0)
                .ShouldHaveEdges(jan, zrobTortille, q1)
                .ShouldHaveEdges(michał, zrobTortille, q1)
                .ShouldHaveEdges(jan, zrobSpaghetti, q2)
                .ShouldHaveEdges(michał, zrobSpaghetti, q0, q2)
                .ShouldHaveEdges(jan, zjedzSpaghetti)
                .ShouldHaveEdges(michał, zjedzSpaghetti)
                .ShouldHaveEdges(jan, zjedzTortille)
                .ShouldHaveEdges(michał, zjedzTortille);

            g.Vertexes.First(p => p.State == q1)
                .ShouldHaveEdges(jan, zrobTortille)
                .ShouldHaveEdges(michał, zrobTortille)
                .ShouldHaveEdges(jan, zrobSpaghetti, q3)
                .ShouldHaveEdges(michał, zrobSpaghetti, q1, q3)
                .ShouldHaveEdges(jan, zjedzSpaghetti)
                .ShouldHaveEdges(michał, zjedzSpaghetti)
                .ShouldHaveEdges(jan, zjedzTortille, q0)
                .ShouldHaveEdges(michał, zjedzTortille, q0);

            g.Vertexes.First(p => p.State == q2)
                .ShouldHaveEdges(jan, zrobTortille, q3)
                .ShouldHaveEdges(michał, zrobTortille, q3)
                .ShouldHaveEdges(jan, zrobSpaghetti)
                .ShouldHaveEdges(michał, zrobSpaghetti)
                .ShouldHaveEdges(jan, zjedzSpaghetti, q0)
                .ShouldHaveEdges(michał, zjedzSpaghetti)
                .ShouldHaveEdges(jan, zjedzTortille)
                .ShouldHaveEdges(michał, zjedzTortille);
                ;

            g.Vertexes.First(p => p.State == q3)
                .ShouldHaveEdges(jan, zrobTortille)
                .ShouldHaveEdges(michał, zrobTortille)
                .ShouldHaveEdges(jan, zrobSpaghetti)
                .ShouldHaveEdges(michał, zrobSpaghetti)
                .ShouldHaveEdges(jan, zjedzSpaghetti, q1)
                .ShouldHaveEdges(michał, zjedzSpaghetti)
                .ShouldHaveEdges(jan, zjedzTortille, q2)
                .ShouldHaveEdges(michał, zjedzTortille, q2);
        }
        
    }
}
