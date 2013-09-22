namespace Omnipaste.OmniClipboard.Infrastructure.Tests.Services.ActivationServiceData.ActivationServiceSteps
{
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData;
    using Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class SaveConfigurationTests
    {
        private SaveConfiguration _subject;

        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void Setup()
        {
            this._mockConfigurationService = new Mock<IConfigurationService>();
            this._subject = new SaveConfiguration(this._mockConfigurationService.Object) { Parameter = new DependencyParameter("channel", "testChannel") };
        }

        [Test]
        public void Execute_Always_ReturnsAResultWithStatusSuccessful()
        {
            this._subject.Execute().State.Should().Be(SingleStateEnum.Successful);
        }

        [Test]
        public void Execute_Always_CallsConfigurationServiceUpdateCommnuicationChannelWithThePayloadObject()
        {
            this._subject.Execute();

            this._mockConfigurationService.Verify(x => x.UpdateCommunicationChannel("testChannel"));
        }
    }
}