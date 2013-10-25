namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class VoidStepTests
    {
        private VoidStep _subject;

        [SetUp]
        public void Setup()
        {
            this._subject = new VoidStep();
        }

        [Test]
        public void ExecuteAlwaysSetsStatusToSuccessful()
        {
            this._subject.Execute().State.Should().Be(SingleStateEnum.Successful);
        }
    }
}