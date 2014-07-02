namespace OmnipasteTests.Services.ActivationServiceData.Transitions
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;
    using Omnipaste.Services.ActivationServiceData.Transitions;

    [TestFixture]
    public class GenericTransitionIdTests
    {
        [Test]
        public void Transitions_WithTheSameState_AreEqual()
        {
            var transtion1 = GenericTransitionId<object>.Create(SimpleStepStateEnum.Successful);
            var transtion2 = GenericTransitionId<object>.Create(SimpleStepStateEnum.Successful);

            transtion1.Should().Be(transtion2);
        }
    }
}