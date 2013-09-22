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
            this._mockConfigurationService = new Mock<IConfigurationService>();
            this._mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(new CommunicationSettings());
            this._subject = new LoadLocalConfiguration(this._mockConfigurationService.Object);
        }

        [Test]
        public void Execute_Always_CallsConfigurationServiceLoadCommunicationSettings()
        {
            this._subject.Execute();

            this._mockConfigurationService.Verify(x => x.Initialize());
        }

        [Test]
        public void Execute_TheChannelOnTheConfigurationServiceIsAValidString_ReturnsResultWithStateSuccessful()
        {
            this._mockConfigurationService.Setup(x => x.CommunicationSettings)
                                     .Returns(new CommunicationSettings { Channel = "test" });

            this._subject.Execute().State.Should().Be(SimpleStepStateEnum.Successful);
        }

        [Test]
        public void Execute_TheChannelOnTheConfigurationServiceIsAnEmptyString_ReturnsResultWithStateFailed()
        {
            this._mockConfigurationService.Setup(x => x.CommunicationSettings)
                                     .Returns(new CommunicationSettings { Channel = string.Empty });

            this._subject.Execute().State.Should().Be(SimpleStepStateEnum.Failed);
        }

        [Test]
        public void Execute_TheChannelOnTheConfigurationServiceIsNull_ReturnsResultWithStateFailed()
        {
            this._mockConfigurationService.Setup(x => x.CommunicationSettings)
                                     .Returns(new CommunicationSettings { Channel = null });

            this._subject.Execute().State.Should().Be(SimpleStepStateEnum.Failed);
        }
    }
}