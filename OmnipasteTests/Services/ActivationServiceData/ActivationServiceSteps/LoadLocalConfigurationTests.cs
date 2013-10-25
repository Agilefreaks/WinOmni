namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using OmniCommon.Services;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

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
        public void ExecuteAlwaysCallsConfigurationServiceLoadCommunicationSettings()
        {
            this._subject.Execute();

            this._mockConfigurationService.Verify(x => x.Initialize());
        }

        [Test]
        public void ExecuteTheChannelOnTheConfigurationServiceIsAValidStringReturnsResultWithStateSuccessful()
        {
            this._mockConfigurationService.Setup(x => x.CommunicationSettings)
                                     .Returns(new CommunicationSettings { Channel = "test" });

            this._subject.Execute().State.Should().Be(SimpleStepStateEnum.Successful);
        }

        [Test]
        public void ExecuteTheChannelOnTheConfigurationServiceIsAnEmptyStringReturnsResultWithStateFailed()
        {
            this._mockConfigurationService.Setup(x => x.CommunicationSettings)
                                     .Returns(new CommunicationSettings { Channel = string.Empty });

            this._subject.Execute().State.Should().Be(SimpleStepStateEnum.Failed);
        }

        [Test]
        public void ExecuteTheChannelOnTheConfigurationServiceIsNullReturnsResultWithStateFailed()
        {
            this._mockConfigurationService.Setup(x => x.CommunicationSettings)
                                     .Returns(new CommunicationSettings { Channel = null });

            this._subject.Execute().State.Should().Be(SimpleStepStateEnum.Failed);
        }
    }
}