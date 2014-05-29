﻿namespace OmniClipboardTests.Messaging
{
    using System;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniClipboard.Messaging;

    [TestFixture]
    public class PubNubMessagingServiceTests
    {
        private PubNubMessagingService _subject;
        private Mock<IPubNubClientFactory> _mockPubNubClientFactory;
        private Mock<IPubNubClient> _mockPubNubClient;

        [SetUp]
        public void SetUp()
        {
            _mockPubNubClientFactory = new Mock<IPubNubClientFactory>();
            _mockPubNubClient = new Mock<IPubNubClient>();
            _mockPubNubClientFactory.Setup(m => m.Create()).Returns(_mockPubNubClient.Object);

            _subject = new PubNubMessagingService(_mockPubNubClientFactory.Object);
        }

        [Test]
        public void InitializeCommunicationsChannelFromConfigurationIsNotEmptyCallsClientFactoryCreate()
        {
            _mockPubNubClient.Setup(
                x => x.Subscribe(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<Action<string>>(), It.IsAny<Action<string>>()))
                      .Callback<string, Action<string>, Action<string>, Action<string>>(
                          (message, dataCallback, statusCallback, errorCallback) => statusCallback("[1, \"Connected\", \"test@email.com\"]"));

            _subject.Connect("test", new Mock<IMessageHandler>().Object);
            
            _mockPubNubClientFactory.Verify(x => x.Create(), Times.Once());
        }

        [Test]
        public void InitializeCommunicationsChannelFromConfigurationServiceIsNotEmptyAndFactoryCreatesAClientWhichReturnsAcorrectStatusMessageSetsIsInitializedTrue()
        {
            _mockPubNubClient.Setup(
                x => x.Subscribe(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<Action<string>>(), It.IsAny<Action<string>>()))
                      .Callback<string, Action<string>, Action<string>, Action<string>>(
                          (message, dataCallback, statusCallback, errorCallback) => statusCallback("[1, \"Connected\", \"test@email.com\"]"));

            _subject.Connect("testChannel", new Mock<IMessageHandler>().Object);
            
            _subject.IsInitialized.Should().BeTrue();
        }
    }
}