using FluentAssertions;
using Moq;
using NUnit.Framework;
using OmniCommon.Interfaces;
using OmniCommon.Services;
using PubNubClipboard;

namespace PubNubClipboardTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using PubNubWrapper;

    using PubNubClipboard = PubNubClipboard.PubNubClipboard;

    [TestFixture]
    public class PubNubClipboardTests
    {
        private Mock<IConfigurationService> _mockConfigurationService;
        private Mock<IPubNubClientFactory> _mockPubNubClientFactory;

        private PubNubClipboard _subject;

        private Mock<IPubNubClient> _mockClient;

        [SetUp]
        public void Setup()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            var communicationSettings = new CommunicationSettings();
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(communicationSettings);
            _mockPubNubClientFactory = new Mock<IPubNubClientFactory> { DefaultValue = DefaultValue.Mock };
            _mockClient = new Mock<IPubNubClient>();
            _mockPubNubClientFactory.Setup(x => x.Create()).Returns(_mockClient.Object);
            _subject = new PubNubClipboard(_mockConfigurationService.Object, _mockPubNubClientFactory.Object);
        }

        [Test]
        public void Initialize_CommunicationsChannelFromConfigurationServiceIsEmpty_SetsIsInitializedFalse()
        {
            var communicationSettings = new CommunicationSettings { Channel = string.Empty };
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(communicationSettings);

            var initializeTask = _subject.Initialize();
            Task.WaitAll(initializeTask);

            _subject.IsInitialized.Should().BeFalse();
        }

        [Test]
        public void Initialize_CommunicationsChannelFromConfigurationIsNotEmpty_CallsClientFactoryCreate()
        {
            SetupCommnuicationSettings();
            _mockPubNubClientFactory.Setup(x => x.Create()).Returns(new Pubnub("test", "test"));

            var initializeTask = _subject.Initialize();
            Task.WaitAll(initializeTask);

            _mockPubNubClientFactory.Verify(x => x.Create(), Times.Once());
        }

        [Test]
        public void Initialize_CommunicationsChannelFromConfigurationServiceIsNotEmptyAndFactoryCreatesAClientWhichReturnsAcorrectStatusMessage_SetsIsInitializedTrue()
        {
            SetupCommnuicationSettings();
            _mockClient.Setup(
                x => x.Subscribe(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<Action<string>>()))
                      .Callback<string, Action<string>, Action<string>>(
                          (message, dataCallback, statusCallback) => statusCallback("[1, \"Connected\", \"test@email.com\"]"));

            var initializeTask = _subject.Initialize();
            Task.WaitAll(initializeTask);

            _subject.IsInitialized.Should().BeTrue();
        }

        [Test]
        public void Initialize_InitializeIsRunningAlready_ReturnsTheSameTaskAsFirstTime()
        {
            SetupCommnuicationSettings();
            _mockClient.Setup(
                x => x.Subscribe(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<Action<string>>()))
                      .Callback<string, Action<string>, Action<string>>(
                          (message, dataCallback, statusCallback) =>
                              {
                                  Thread.Sleep(500);
                                  statusCallback("[1, \"Connected\", \"test@email.com\"]");
                              });

            var initializeTask = _subject.Initialize();
            var secondTask = _subject.Initialize();

            initializeTask.Should().BeSameAs(secondTask);
        }

        private void SetupCommnuicationSettings()
        {
            var settings = new CommunicationSettings { Channel = "asd" };
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(settings);
        }
    }
}
