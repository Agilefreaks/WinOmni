namespace Omnipaste.OmniClipboard.Infrastructure.Tests.Services.ActivationServiceData.ActivationServiceSteps
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps;

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
        public void Execute_Always_SetsStatusToSuccessful()
        {
            this._subject.Execute().State.Should().Be(SingleStateEnum.Successful);
        }
    }
}