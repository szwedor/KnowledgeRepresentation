namespace Stories.Tests
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;

    public class NonvalidInputTests
    {

        [Test]
        public void UsingKeywordsForActionShouldFail()
        {
            Action action = () => $"or causes duckAlive".GetHistory();
            action.Should().Throw<Exception>().WithMessage("Parsing history error");
        }
        [Test]
        public void UsingKeywordsForConditionShouldFail()
        {

            Action action = () => $"Shoot causes or".GetHistory();
            action.Should().Throw<Exception>().WithMessage("Parsing history error");
        }
        [Test]
        public void UsingKeywordsForActorShouldFail()
        {

            Action action = () => $"when or Shot causes duckAlive".GetHistory();
            action.Should().Throw<Exception>().WithMessage("Parsing history error");
        }

        [Test]
        public void OnMissingLogicOperatorsShouldFail()
        {

            Action action = () => $"always duckAlive duckFly".GetHistory();
            action.Should().Throw<Exception>().WithMessage("Parsing history error");


        }
    }
}
