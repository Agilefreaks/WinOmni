using ClipboardWatcher.Core;
using ClipboardWatcher.Core.Impl.PubNub;
using ClipboardWatcher.Core.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CloudClipboardTests
{
    [TestFixture]
    public class PubNubCloudClipboardTests
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

            var cloudClipboard = new PubNubCloudClipboard(_mockConfigurationService.Object, _mockPubNubClientFactory.Object);

            cloudClipboard.IsInitialized.Should().BeFalse();
        }

        [Test]
        public void Ctor_CommunicationsChannelFromConfigurationIsNotEmpty_CallsClientFactoryCreate()
        {
            var settings = new CommunicationSettings { Channel = "asd" };
            var communicationSettings = settings;
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(communicationSettings);
            _mockPubNubClientFactory.Setup(x => x.Create(settings)).Returns(new Pubnub("test", "test"));

            // ReSharper disable ObjectCreationAsStatement
            new PubNubCloudClipboard(_mockConfigurationService.Object, _mockPubNubClientFactory.Object);
            // ReSharper restore ObjectCreationAsStatement

            _mockPubNubClientFactory.Verify(x => x.Create(settings), Times.Once());
        }

        [Test]
        public void Ctor_CommunicationsChannelFromConfigurationServiceIsNotEmptyAndFactoryCreatesAClient_SetsIsInitializedTrue()
        {
            var communicationSettings = new CommunicationSettings { Channel = "asda" };
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(communicationSettings);
            _mockPubNubClientFactory.Setup(x => x.Create(It.IsAny<CommunicationSettings>())).Returns(new Pubnub("test", "test"));

            var cloudClipboard = new PubNubCloudClipboard(_mockConfigurationService.Object, _mockPubNubClientFactory.Object);

            cloudClipboard.IsInitialized.Should().BeTrue();
        }
    }
}
