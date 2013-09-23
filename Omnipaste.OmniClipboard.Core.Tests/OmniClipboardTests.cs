namespace Omnipaste.OmniClipboard.Core.Tests
{
    using System;
    using Omnipaste.OmniClipboard.Core.Api;
    using Omnipaste.OmniClipboard.Core.Api.Resources;
    using Omnipaste.OmniClipboard.Core.Messaging;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Logging;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using OmniCommon.Services;

    [TestFixture]
    public class OmniClipboardTests
    {
        private Mock<IConfigurationService> _mockConfigurationService;

        private OmniClipboard _subject;

        private Mock<ILog> _mockLogger;

        private Mock<IMessagingService> _mockMessagingService;

        private Mock<IClippings> _mockClippings;

        [SetUp]
        public void Setup()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            var communicationSettings = new CommunicationSettings();
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(communicationSettings);
            _mockLogger = new Mock<ILog>();
            _mockMessagingService = new Mock<IMessagingService>();
            _mockClippings = new Mock<IClippings>();

            _subject = new OmniClipboard(_mockConfigurationService.Object, _mockClippings.Object, _mockMessagingService.Object)
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
        public void Initialize_Always_SetsOmniApiKey()
        {
            SetupCommnuicationSettings();
            _mockMessagingService.Setup(
                x => x.Connect(It.IsAny<string>(), It.IsAny<IMessageHandler>()))
                      .Callback<string, IMessageHandler>((message, handler) => Thread.Sleep(500));

            var initializeTask = _subject.Initialize();
            
            initializeTask.Wait();
            _mockClippings.VerifySet(m => m.ApiKey = "test");
        }

        [Test]
        public void NewMessageReceived_WhenTheMessageWasMyOwn_WillNotGetTheLastClippingSinceIAlreadyHaveIt()
        {
            string messageGuid = Guid.NewGuid().ToString();
            _subject.MessageGuid = messageGuid;
            _mockMessagingService.Setup(
                m => m.Connect(It.IsAny<string>(), It.IsAny<IMessageHandler>()))
                       .Callback<string, IMessageHandler>(
                           (message, handler) => handler.MessageReceived(messageGuid));
            InitializeMockClient();

            _subject.Initialize();

            _mockClippings.Verify(m => m.GetLastAsync(_subject), Times.Exactly(0));
        }

        [Test]
        public void NewMessageReceived_Always_GetsClippingFromApi()
        {
            string messageGuid = Guid.NewGuid().ToString();
            _subject.MessageGuid = messageGuid;
            _mockMessagingService.Setup(x => x.Connect(It.IsAny<string>(), It.IsAny<IMessageHandler>()))
                       .Callback<string, IMessageHandler>((p1, p2) => p2.MessageReceived(Guid.NewGuid().ToString()));
            InitializeMockClient();

            _subject.Initialize();

            _mockClippings.Verify(m => m.GetLastAsync(_subject));
        }

        [Test]
        public void PutData_Always_CallsApiSaveClippingAsync()
        {
            InitializeMockClient();

            _subject.PutData("data");

            _mockClippings.Verify(m => m.SaveAsync("data", _subject));
        }

        [Test]
        public void SaveClippingSucceeded_Always_CallsPubNubPublishWithNewMessage()
        {
            InitializeMockClient();

            ((ISaveClippingCompleteHandler)_subject).SaveClippingSucceeded();

            _mockMessagingService.Verify(m => m.SendAsync(It.IsAny<string>(), "NewMessage", It.IsAny<IMessageHandler>()));
        }

        [Test]
        public void SaveClippingSucceeded_Always_SetsAnotherGuid()
        {
            InitializeMockClient();
            var previousGuid = _subject.MessageGuid = Guid.NewGuid().ToString();

            ((ISaveClippingCompleteHandler)_subject).SaveClippingSucceeded();

            Assert.AreNotEqual(previousGuid, _subject.MessageGuid);
        }

        private void InitializeMockClient()
        {
            SetupCommnuicationSettings();
            var initializeTask = _subject.Initialize();
            Task.WaitAll(initializeTask);
        }

        private void SetupCommnuicationSettings()
        {
            var settings = new CommunicationSettings { Channel = "test" };
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(settings);
        }
    }
}
