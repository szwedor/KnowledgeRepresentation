namespace Stories.Tests
{
    using FluentAssertions;
    using NUnit.Framework;

    public class NonInertialStatementTests
    {
        [Test]
        public void NonIntertialTest()
        {
            var history = $"noninertial duckAlive".GetHistory();
            history.NonInertial.Count.Should().Be(1);
            history.NonInertial[0].Fluent.Should().Be("duckAlive");
        }
    }
}
