namespace Stories.Tests
{
    using FluentAssertions;
    using NUnit.Framework;
    using Stories.Parser.Conditions;

    [TestFixtureSource(typeof(ConditionsTestData), "Data")]
    public class RequirementTests
    {
        private readonly ConditionExpression expression;
        private readonly string condition;

        public RequirementTests(string condition, ConditionExpression expression)
        {
            this.condition = condition;
            this.expression = expression;
        }
        [Test]
        public void AlwaysTest()
        {
            var history = $"always {this.condition}".GetHistory();
            history.Always.Count.Should().Be(1);
            history.Always[0].Condition.ShouldEqualCondition(this.expression);
        }
    }
}
