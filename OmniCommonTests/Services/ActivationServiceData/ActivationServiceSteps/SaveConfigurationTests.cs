namespace OmniCommonTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class SaveConfigurationTests
    {
        private SaveConfiguration _subject;

        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void Setup()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            _subject = new SaveConfiguration(_mockConfigurationService.Object, "testChannel");
        }

        [Test]
        public void Execute_Always_ReturnsAResultWithStatusSuccessful()
        {
            _subject.Execute().State.Should().Be(SingleStateEnum.Successful);
        }

        [Test]
        public void Execute_Always_CallsConfigurationServiceUpdateCommnuicationChannelWithThePayloadObject()
        {
            _subject.Execute();

            _mockConfigurationService.Verify(x => x.UpdateCommunicationChannel("testChannel"));
        }
    }
}