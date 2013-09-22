namespace Omnipaste.OmniClipboard.Infrastructure.Tests.Services.ActivationServiceData.ActivationServiceSteps
{
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using OmniCommon.Services;
    using Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class LoadLocalConfigurationTests
    {
        private LoadLocalConfiguration _subject;

        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void Setup()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(new CommunicationSettings());
            _subject = new LoadLocalConfiguration(_mockConfigurationService.Object);
        }

        [Test]
        public void Execute_Always_CallsConfigurationServiceLoadCommunicationSettings()
        {
            _subject.Execute();

            _mockConfigurationService.Verify(x => x.Initialize());
        }

        [Test]
        public void Execute_TheChannelOnTheConfigurationServiceIsAValidString_ReturnsResultWithStateSuccessful()
        {
            _mockConfigurationService.Setup(x => x.CommunicationSettings)
                                     .Returns(new CommunicationSettings { Channel = "test" });

            _subject.Execute().State.Should().Be(SimpleStepStateEnum.Successful);
        }

        [Test]
        public void Execute_TheChannelOnTheConfigurationServiceIsAnEmptyString_ReturnsResultWithStateFailed()
        {
            _mockConfigurationService.Setup(x => x.CommunicationSettings)
                                     .Returns(new CommunicationSettings { Channel = string.Empty });

            _subject.Execute().State.Should().Be(SimpleStepStateEnum.Failed);
        }

        [Test]
        public void Execute_TheChannelOnTheConfigurationServiceIsNull_ReturnsResultWithStateFailed()
        {
            _mockConfigurationService.Setup(x => x.CommunicationSettings)
                                     .Returns(new CommunicationSettings { Channel = null });

            _subject.Execute().State.Should().Be(SimpleStepStateEnum.Failed);
        }
    }
}