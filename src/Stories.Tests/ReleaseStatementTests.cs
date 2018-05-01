namespace Stories.Tests
{
    using FluentAssertions;
    using NUnit.Framework;
    using Stories.Parser.Conditions;

    [TestFixtureSource(typeof(ConditionsTestData), "Data")]

    public class ReleaseStatementTests
    {
        private readonly string condition;
        private readonly ConditionExpression expression;

        public ReleaseStatementTests(string condition, ConditionExpression expression)
        {
            this.condition = condition;
            this.expression = expression;
        }
        [Test]
        public void ReleasesTest()
        {
            var history = "Shoot releases DuckAlive".GetHistory();
            history.Releases.Count.Should().Be(1);
            var release = history.Releases[0];
            release.Fluent.Should().Be("DuckAlive");
            release.Agents.Should().BeNull();
            release.Action.Should().Be("Shoot");
            release.Condition.ShouldEqualCondition(new ConditionConstant(true));
            release.IsNegated.Should().BeFalse();
        }
        [Test]
        public void ReleasesTestWithPrecondition()
        {
            var history = $"Shoot releases DuckAlive if {this.condition}".GetHistory();
            history.Releases.Count.Should().Be(1);
            var release = history.Releases[0];
            release.Fluent.Should().Be("DuckAlive");
            release.Agents.Should().BeNull();
            release.Action.Should().Be("Shoot");
            release.Condition.ShouldEqualCondition(this.expression);
            release.IsNegated.Should().BeFalse();
        }

        [Test]
        public void ReleasesTestWithActor()
        {
            var history = "when John Shoot releases DuckAlive".GetHistory();
            history.Releases.Count.Should().Be(1);
            var release = history.Releases[0];
            release.Fluent.Should().Be("DuckAlive");
            release.Agents.Should().BeEquivalentTo("John");
            release.Action.Should().Be("Shoot");
            release.Condition.ShouldEqualCondition(new ConditionConstant(true));
            release.IsNegated.Should().BeFalse();
        }
        [Test]
        public void ReleasesTestWithActorAndPrecondition()
        {
            var history = $"when John Shoot releases DuckAlive if {this.condition}".GetHistory();
            history.Releases.Count.Should().Be(1);
            var release = history.Releases[0];
            release.Fluent.Should().Be("DuckAlive");
            release.Agents.Should().BeEquivalentTo("John");
            release.Action.Should().Be("Shoot");
            release.Condition.ShouldEqualCondition(this.expression);
            release.IsNegated.Should().BeFalse();
        }

        [Test]
        public void ReleasesTestWithActors()
        {
            var history = "when John or Izzack Shoot releases DuckAlive".GetHistory();
            history.Releases.Count.Should().Be(1);
            var release = history.Releases[0];
            release.Fluent.Should().Be("DuckAlive");
            release.Agents.Should().BeEquivalentTo("John", "Izzack");
            release.Action.Should().Be("Shoot");
            release.Condition.ShouldEqualCondition(new ConditionConstant(true));
            release.IsNegated.Should().BeFalse();
        }
        [Test]
        public void ReleasesTestWithActorsAndPrecondition()
        {
            var history = $"when John or Izzack Shoot releases DuckAlive if {this.condition}".GetHistory();
            history.Releases.Count.Should().Be(1);
            var release = history.Releases[0];
            release.Fluent.Should().Be("DuckAlive");
            release.Agents.Should().BeEquivalentTo("John", "Izzack");
            release.Action.Should().Be("Shoot");
            release.Condition.ShouldEqualCondition(this.expression);
            release.IsNegated.Should().BeFalse();
        }
    }
}
