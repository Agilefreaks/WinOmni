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

        [SetUp]
        public void Setup()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            var communicationSettings = new CommunicationSettings();
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(communicationSettings);
            _mockPubNubClientFactory = new Mock<IPubNubClientFactory>();
        }

        [Test]
        public void Ctor_CommunicationsChannelFromConfigurationServiceIsEmpty_SetsIsInitializedFalse()
        {
            var communicationSettings = new CommunicationSettings { Channel = string.Empty };
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(communicationSettings);

            var omniclipboard = new PubNubClipboard(_mockConfigurationService.Object, _mockPubNubClientFactory.Object);

            omniclipboard.IsInitialized.Should().BeFalse();
        }

        [Test]
        public void Ctor_CommunicationsChannelFromConfigurationIsNotEmpty_CallsClientFactoryCreate()
        {
            var settings = new CommunicationSettings { Channel = "asd" };
            var communicationSettings = settings;
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(communicationSettings);
            _mockPubNubClientFactory.Setup(x => x.Create()).Returns(new Pubnub("test", "test"));

            var pubNubClipboard = new PubNubClipboard(_mockConfigurationService.Object, _mockPubNubClientFactory.Object);

            pubNubClipboard.Should().NotBeNull();
            _mockPubNubClientFactory.Verify(x => x.Create(), Times.Once());
        }

        [Test]
        public void Ctor_CommunicationsChannelFromConfigurationServiceIsNotEmptyAndFactoryCreatesAClient_SetsIsInitializedTrue()
        {
            var communicationSettings = new CommunicationSettings { Channel = "asda" };
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(communicationSettings);
            _mockPubNubClientFactory.Setup(x => x.Create()).Returns(new Pubnub("test", "test"));

            var omniclipboard = new PubNubClipboard(_mockConfigurationService.Object, _mockPubNubClientFactory.Object);

            omniclipboard.IsInitialized.Should().BeTrue();
        }
    }
}
