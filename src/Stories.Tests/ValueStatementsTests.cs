namespace Stories.Tests
{
    using FluentAssertions;
    using Stories.Parser.Conditions;
    using Stories.Parser.Statements;
    using NUnit.Framework;

    [TestFixtureSource(typeof(ConditionsTestData), "ObservableData")]
    public class ValueStatements
    {
        private readonly string condition;
        private readonly ConditionExpression conditionExpression;
        private readonly bool observable;
        private string ObservableString => this.observable ? "observable " : string.Empty;
        public ValueStatements(string condition, ConditionExpression conditionExpression, bool observable)
        {
            this.condition = condition;
            this.conditionExpression = conditionExpression;
            this.observable = observable;
        }
        private void AlwaysEffectsNoninertialReleasesShouldBeEmpty(HistoryStatement history)
        {
            history.Always.Should().BeEmpty();
            history.Effects.Should().BeEmpty();
            history.NonInertial.Should().BeEmpty();
            history.Releases.Should().BeEmpty();
        }

         [Test]
        public void AfterStatementWithOneAction()
         {
             var history = $"{this.ObservableString}{this.condition} after AttackBilbo".GetHistory();

             history.Values.Count.Should().Be(1);
             var value = history.Values[0];

             value.Condition.ShouldEqualCondition(this.conditionExpression);
             value.IsObservable.Should().Be(this.observable);
             value.Actions.Should().OnlyContain(p => "AttackBilbo" == p.Action && null == p.Agent);

             this.AlwaysEffectsNoninertialReleasesShouldBeEmpty(history);
         }

        
        [Test]
        public void AfterStatementWithOneActionWithActor()
        {
            var history = $"{this.ObservableString}{this.condition} after Bilbo AttackBilbo".GetHistory();

            history.Values.Count.Should().Be(1);
            var value = history.Values[0];

            value.Condition.ShouldEqualCondition(this.conditionExpression);
            value.IsObservable.Should().Be(this.observable);
            value.Actions.Should().OnlyContain(p => "AttackBilbo" == p.Action && "Bilbo" == p.Agent);

            this.AlwaysEffectsNoninertialReleasesShouldBeEmpty(history);
        }

        [Test]
        public void AfterStatementWithActions()
        {
            var history = $"{this.ObservableString}{this.condition} after AttackBilbo, LookAtOrcs".GetHistory();

            history.Values.Count.Should().Be(1);
            var value = history.Values[0];

            value.Condition.ShouldEqualCondition(this.conditionExpression);
            value.IsObservable.Should().Be(this.observable);

            value.Actions.Should().BeEquivalentTo(
                new ActionWithExecutor() { Action = "AttackBilbo", Agent = null}, 
                new ActionWithExecutor() { Action = "LookAtOrcs", Agent = null }
                );

            this.AlwaysEffectsNoninertialReleasesShouldBeEmpty(history);
        }

        [Test]
        public void AfterStatementWithActionsWithActors()
        {
            var history = $"{this.ObservableString}{this.condition} after Bilbo AttackBilbo, Frodo LookAtOrcs".GetHistory();

            history.Values.Count.Should().Be(1);
            var value = history.Values[0];

            value.Condition.ShouldEqualCondition(this.conditionExpression);
            value.IsObservable.Should().Be(this.observable);

            value.Actions.Should().BeEquivalentTo(
                new ActionWithExecutor() { Action = "AttackBilbo", Agent = "Bilbo" },
                new ActionWithExecutor() { Action = "LookAtOrcs", Agent = "Frodo" }
            );

            this.AlwaysEffectsNoninertialReleasesShouldBeEmpty(history);
        }

        [Test]
        public void AfterStatementWithActionsPartiallyWithActors()
        {
            var history = $"{this.ObservableString}{this.condition} after AttackBilbo, Frodo LookAtOrcs".GetHistory();

            history.Values.Count.Should().Be(1);
            var value = history.Values[0];

            value.Condition.ShouldEqualCondition(this.conditionExpression);
            value.IsObservable.Should().Be(this.observable);

            value.Actions.Should().BeEquivalentTo(
                new ActionWithExecutor() { Action = "AttackBilbo", Agent = null },
                new ActionWithExecutor() { Action = "LookAtOrcs", Agent = "Frodo" }
            );

            this.AlwaysEffectsNoninertialReleasesShouldBeEmpty(history);
        }
        [Test]
        public void Initially()
        {
            var history = $"initially {this.condition}".GetHistory();
            history.Values.Count.Should().Be(1);
            var value = history.Values[0];

            value.Condition.ShouldEqualCondition(this.conditionExpression);
            value.IsObservable.Should().BeFalse();

            value.Actions.Should().BeEmpty();

            this.AlwaysEffectsNoninertialReleasesShouldBeEmpty(history);

        }
    }
}
