namespace Stories.Tests
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using Stories.Parser.Conditions;
    using Stories.Parser.Statements;

    [TestFixtureSource(typeof(ConditionsTestData), "EffectAndPreconditionTypically")]
    public class EffectStatementTests
    {
        private readonly string effectCondition;
        private readonly ConditionExpression effectConditionExpression;
        private readonly string preCondition;
        private readonly ConditionExpression preConditionExpression;
        private readonly bool typically;
        private string TypicallyString => this.typically ? "typically " : string.Empty;

        public EffectStatementTests(string effectCondition,
            ConditionExpression effectConditionExpression, 
            string preCondition, 
            ConditionExpression preConditionExpression,
            bool typically)
        {
            this.effectCondition = effectCondition;
            this.effectConditionExpression = effectConditionExpression;
            this.preCondition = preCondition;
            this.preConditionExpression = preConditionExpression;
            this.typically = typically;
        }
        private void AlwaysValuesNoninertialReleasesShouldBeEmpty(HistoryStatement history)
        {
            history.Always.Should().BeEmpty();
            history.Values.Should().BeEmpty();
            history.NonInertial.Should().BeEmpty();
            history.Releases.Should().BeEmpty();
        }


        [Test]
        public void CausesStatement()
        {
            var history = $"Shoot {this.TypicallyString}causes {this.effectCondition}".GetHistory();

            history.Effects.Count.Should().Be(1);
            var effect = history.Effects[0];

            effect.Action.Should().Be("Shoot");
            effect.Agents.Should().BeNull();
            effect.IsTypical.Should().Be(this.typically);
            effect.Condition.ShouldEqualCondition(new ConditionConstant(true));
            effect.Effect.ShouldEqualCondition(this.effectConditionExpression);

            this.AlwaysValuesNoninertialReleasesShouldBeEmpty(history);
        }

        [Test]
        public void CausesStatementWithPrecondition()
        {
            var history = $"Shoot {this.TypicallyString}causes {this.effectCondition} if {this.preCondition}".GetHistory();
            history.Effects.Count.Should().Be(1);
            var effect = history.Effects[0];

            effect.Action.Should().Be("Shoot");
            effect.Agents.Should().BeNull();
            effect.IsTypical.Should().Be(this.typically);
            effect.Condition.ShouldEqualCondition(this.preConditionExpression);
            effect.Effect.ShouldEqualCondition(this.effectConditionExpression);

            this.AlwaysValuesNoninertialReleasesShouldBeEmpty(history);
        }
        [Test]
        public void CausesStatementWithPreconditionAndActor()
        {
            var history = $"when John Shoot {this.TypicallyString}causes {this.effectCondition} if {this.preCondition}".GetHistory();
            history.Effects.Count.Should().Be(1);
            var effect = history.Effects[0];

            effect.Action.Should().Be("Shoot");
            effect.Agents.Should().BeEquivalentTo("John");
            effect.IsTypical.Should().Be(this.typically);
            effect.Condition.ShouldEqualCondition(this.preConditionExpression);
            effect.Effect.ShouldEqualCondition(this.effectConditionExpression);

            this.AlwaysValuesNoninertialReleasesShouldBeEmpty(history);
        }
        [Test]
        public void CausesStatementWithPreconditionAndActors()
        {
            var history = $"when John or Jeny Shoot {this.TypicallyString}causes {this.effectCondition} if {this.preCondition}".GetHistory();
            history.Effects.Count.Should().Be(1);
            var effect = history.Effects[0];

            effect.Action.Should().Be("Shoot");
            effect.Agents.Should().BeEquivalentTo("John","Jeny");
            effect.IsTypical.Should().Be(this.typically);
            effect.Condition.ShouldEqualCondition(this.preConditionExpression);
            effect.Effect.ShouldEqualCondition(this.effectConditionExpression);

            this.AlwaysValuesNoninertialReleasesShouldBeEmpty(history);
        }
        [Test]
        public void CausesStatementWithActor()
        {
            var history = $"when John Shoot {this.TypicallyString}causes {this.effectCondition}".GetHistory();
            history.Effects.Count.Should().Be(1);
            var effect = history.Effects[0];

            effect.Action.Should().Be("Shoot");
            effect.Agents.Should().BeEquivalentTo("John");
            effect.IsTypical.Should().Be(this.typically);
            effect.Condition.ShouldEqualCondition(new ConditionConstant(true));
            effect.Effect.ShouldEqualCondition(this.effectConditionExpression);

            this.AlwaysValuesNoninertialReleasesShouldBeEmpty(history);
        }
        [Test]
        public void CausesStatementWithActors()
        {
            var history = $"when John or Smith Shoot {this.TypicallyString}causes {this.effectCondition}".GetHistory();
            history.Effects.Count.Should().Be(1);
            var effect = history.Effects[0];

            effect.Action.Should().Be("Shoot");
            effect.Agents.Should().BeEquivalentTo("John","Smith");
            effect.IsTypical.Should().Be(this.typically);
            effect.Condition.ShouldEqualCondition(new ConditionConstant(true));
            effect.Effect.ShouldEqualCondition(this.effectConditionExpression);

            this.AlwaysValuesNoninertialReleasesShouldBeEmpty(history);
        }

        [Test]
        public void ImpossibleWithPrecondition()
        {
            var history = $"impossible Shoot if {this.preCondition}".GetHistory();
            history.Effects.Count.Should().Be(1);
            var effect = history.Effects[0];

            effect.Action.Should().Be("Shoot");
            effect.Agents.Should().BeNull();
            effect.Condition.ShouldEqualCondition(this.preConditionExpression);
            effect.Effect.ShouldEqualCondition(new ConditionConstant(false));

            this.AlwaysValuesNoninertialReleasesShouldBeEmpty(history);
        }

        [Test]
        public void ImpossibleWithActor()
        {
            var history = $"impossible John Shoot".GetHistory();
            history.Effects.Count.Should().Be(1);
            var effect = history.Effects[0];

            effect.Action.Should().Be("Shoot");
            effect.Agents.Should().BeEquivalentTo("John");
            effect.Condition.ShouldEqualCondition(new ConditionConstant(true));
            effect.Effect.ShouldEqualCondition(new ConditionConstant(false));

            this.AlwaysValuesNoninertialReleasesShouldBeEmpty(history);
        }

        [Test]
        public void ImpossibleWithActors()
        {
            var history = $"impossible John or Jeny Shoot".GetHistory();
            history.Effects.Count.Should().Be(1);
            var effect = history.Effects[0];

            effect.Action.Should().Be("Shoot");
            effect.Agents.Should().BeEquivalentTo("John","Jeny");
            effect.Condition.ShouldEqualCondition(new ConditionConstant(true));
            effect.Effect.ShouldEqualCondition(new ConditionConstant(false));

            this.AlwaysValuesNoninertialReleasesShouldBeEmpty(history);
        }

        [Test]
        public void ImpossibleWithActorAndPrecondition()
        {
            var history = $"impossible John Shoot if {this.preCondition}".GetHistory();
            history.Effects.Count.Should().Be(1);
            var effect = history.Effects[0];

            effect.Action.Should().Be("Shoot");
            effect.Agents.Should().BeEquivalentTo("John");
            effect.Condition.ShouldEqualCondition(this.preConditionExpression);
            effect.Effect.ShouldEqualCondition(new ConditionConstant(false));

            this.AlwaysValuesNoninertialReleasesShouldBeEmpty(history);
        }

        [Test]
        public void ImpossibleWithActorsAndPrecondition()
        {
            var history = $"impossible John or Jeny Shoot if {this.preCondition}".GetHistory();
            history.Effects.Count.Should().Be(1);
            var effect = history.Effects[0];

            effect.Action.Should().Be("Shoot");
            effect.Agents.Should().BeEquivalentTo("John","Jeny");
            effect.Condition.ShouldEqualCondition(this.preConditionExpression);
            effect.Effect.ShouldEqualCondition(new ConditionConstant(false));

            this.AlwaysValuesNoninertialReleasesShouldBeEmpty(history);
        }
    }
}
