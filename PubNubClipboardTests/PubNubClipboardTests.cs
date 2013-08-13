using Omnipaste.OmniClipboard.Core;
using Omnipaste.OmniClipboard.Core.Api;
using Omnipaste.OmniClipboard.Core.Messaging;

namespace PubNubClipboardTests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Logging;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using OmniCommon.Services;

    [TestFixture]
    public class PubNubClipboardTests
    {
        private Mock<IConfigurationService> _mockConfigurationService;

        private OmniClipboard _subject;

        private Mock<ILog> _mockLogger;

        private Mock<IOmniApi> _mockOmniApi;

        private Mock<IMessagingService> _mockMessagingService;

        [SetUp]
        public void Setup()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            var communicationSettings = new CommunicationSettings();
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(communicationSettings);
            _mockLogger = new Mock<ILog>();
            _mockOmniApi = new Mock<IOmniApi>();
            _mockMessagingService = new Mock<IMessagingService>();
            _subject = new OmniClipboard(_mockConfigurationService.Object, _mockOmniApi.Object, _mockMessagingService.Object)
                           {
                               Logger = _mockLogger.Object
                           };
        }

        [Test]
        public void Initialize_IsEmpty_SetsIsInitializedFalse()
        {
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(new CommunicationSettings { Channel = string.Empty });

            var initializeTask = _subject.Initialize();
            Task.WaitAll(initializeTask);

            _mockMessagingService.Verify(m => m.Connect(It.IsAny<string>(), It.IsAny<IMessageHandler>()), Times.Never());
        }

        [Test]
        public void Initialize_InitializeIsRunningAlready_ReturnsTheSameTaskAsFirstTime()
        {
            SetupCommnuicationSettings();
            _mockMessagingService.Setup(
                x => x.Connect(It.IsAny<string>(), It.IsAny<IMessageHandler>()))
                      .Callback<string, IMessageHandler>((message, handler) => Thread.Sleep(500));

            var initializeTask = _subject.Initialize();
            var secondTask = _subject.Initialize();

            initializeTask.Should().BeSameAs(secondTask);
        }

        [Test]
        public void NewMessageReceived_Always_GetsClippingFromApi()
        {
            InitializeMockClient();
            _mockMessagingService.Setup(
                m => m.Connect(It.IsAny<string>(), It.IsAny<IMessageHandler>()))
                       .Callback<string, IMessageHandler>(
                           (message, handler) => handler.MessageReceived("test"));

            _subject.Initialize();

            _mockOmniApi.Verify(m => m.GetLastClippingAsync(_subject));
        }

        [Test]
        public void PutData_Always_CallsApiSaveClippingAsync()
        {
            InitializeMockClient();

            _subject.PutData("data");

            _mockOmniApi.Verify(m => m.SaveClippingAsync("data", _subject));
        }

        [Test]
        public void SaveClippingSucceeded_Always_CallsPubNubPublishWithNewMessage()
        {
            InitializeMockClient();

            ((ISaveClippingCompleteHandler)_subject).SaveClippingSucceeded();

            _mockMessagingService.Verify(m => m.SendAsync(It.IsAny<string>(), "NewMessage", It.IsAny<IMessageHandler>()));
        }

        private void InitializeMockClient()
        {
            SetupCommnuicationSettings();
            _mockMessagingService.Setup(x => x.Connect(It.IsAny<string>(), It.IsAny<IMessageHandler>()))
                       .Callback<string, IMessageHandler>((p1, p2) => p2.MessageReceived("[1, \"test\", \"test\"]"));
            var initializeTask = _subject.Initialize();
            Task.WaitAll(initializeTask);
        }

        private void SetupCommnuicationSettings()
        {
            var settings = new CommunicationSettings { Channel = "asd" };
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(settings);
        }
    }
}
