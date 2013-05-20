using FluentAssertions;
using Moq;
using NUnit.Framework;
using OmniCommon.Interfaces;
using OmniCommon.Services;
using PubNubClipboard;

namespace PubNubClipboardTests
{
    using PubNubClipboard = PubNubClipboard.PubNubClipboard;

    [TestFixture]
    public class PubNubClipboardTests
    {
        private Mock<IConfigurationService> _mockConfigurationService;
        private Mock<IPubNubClientFactory> _mockPubNubClientFactory;

        private PubNubClipboard _subject;

        [SetUp]
        public void Setup()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            var communicationSettings = new CommunicationSettings();
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(communicationSettings);
            _mockPubNubClientFactory = new Mock<IPubNubClientFactory>();
            this._subject = new PubNubClipboard(_mockConfigurationService.Object, _mockPubNubClientFactory.Object);
        }

        [Test]
        public void Initialize_CommunicationsChannelFromConfigurationServiceIsEmpty_SetsIsInitializedFalse()
        {
            var communicationSettings = new CommunicationSettings { Channel = string.Empty };
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(communicationSettings);

            _subject.Initialize();

            _subject.IsInitialized.Should().BeFalse();
        }

        [Test]
        public void Initialize_CommunicationsChannelFromConfigurationIsNotEmpty_CallsClientFactoryCreate()
        {
            var settings = new CommunicationSettings { Channel = "asd" };
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(settings);
            _mockPubNubClientFactory.Setup(x => x.Create()).Returns(new Pubnub("test", "test"));

            _subject.Initialize();

            _mockPubNubClientFactory.Verify(x => x.Create(), Times.Once());
        }

        [Test]
        public void Initialize_CommunicationsChannelFromConfigurationServiceIsNotEmptyAndFactoryCreatesAClient_SetsIsInitializedTrue()
        {
            var communicationSettings = new CommunicationSettings { Channel = "asda" };
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(communicationSettings);
            _mockPubNubClientFactory.Setup(x => x.Create()).Returns(new Pubnub("test", "test"));

            _subject.Initialize();

            _subject.IsInitialized.Should().BeTrue();
        }
    }
}
