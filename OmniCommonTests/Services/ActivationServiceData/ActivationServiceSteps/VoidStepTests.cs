namespace OmniCommonTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class VoidStepTests
    {
        private VoidStep _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new VoidStep();
        }

        [Test]
        public void Execute_Always_SetsStatusToSuccessful()
        {
            _subject.Execute().State.Should().Be(SingleStateEnum.Successful);
        }
    }
}