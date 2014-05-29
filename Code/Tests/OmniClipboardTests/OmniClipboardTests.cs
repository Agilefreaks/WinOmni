﻿namespace OmniClipboardTests
 {
     using System;
     using System.Threading;
     using System.Threading.Tasks;
     using FluentAssertions;
     using Moq;
     using NUnit.Framework;
     using OmniApi.Resources;
     using OmniClipboard.Messaging;
     using global::OmniClipboard;
     using OmniCommon.Interfaces;
     using OmniCommon.Services;
     using Omnipaste.OmniClipboard.Core.Api;

     [TestFixture]
     public class OmniClipboardTests
     {
         private Mock<IConfigurationService> _mockConfigurationService;

         private Mock<IMessagingService> _mockMessagingService;

         private Mock<IClippings> _mockClippings;

         private OmniClipboard _subject;

         [SetUp]
         public void Setup()
         {
             _mockConfigurationService = new Mock<IConfigurationService>();
             var communicationSettings = new CommunicationSettings();
             _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(communicationSettings);
             _mockMessagingService = new Mock<IMessagingService>();
             _mockClippings = new Mock<IClippings>();

             _subject = new OmniClipboard(_mockConfigurationService.Object, _mockMessagingService.Object)
                            {
                                Clippings = _mockClippings.Object
                            };
         }

         [Test]
         public void InitializeIsEmptySetsIsInitializedFalse()
         {
             _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(new CommunicationSettings { Channel = string.Empty });

             var initializeTask = _subject.Initialize();
             Task.WaitAll(initializeTask);

             _mockMessagingService.Verify(m => m.Connect(It.IsAny<string>(), It.IsAny<IMessageHandler>()), Times.Never());
         }

         [Test]
         public void InitializeInitializeIsRunningAlreadyReturnsTheSameTaskAsFirstTime()
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
         public void InitializeAlwaysSetsOmniApiChannel()
         {
             SetupCommnuicationSettings();
             _mockMessagingService.Setup(
                 x => x.Connect(It.IsAny<string>(), It.IsAny<IMessageHandler>()))
                       .Callback<string, IMessageHandler>((message, handler) => Thread.Sleep(500));

             var initializeTask = _subject.Initialize();
             initializeTask.Wait();

             _mockClippings.VerifySet(m => m.Channel = "test");
         }

         [Test]
         public void NewMessageReceivedWhenTheMessageWasMyOwnWillNotGetTheLastClippingSinceIAlreadyHaveIt()
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
         public void NewMessageReceivedAlwaysGetsClippingFromApi()
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
         public void NewMessageReceivedWithMessageGuidNullGetsMessageFromApi()
         {
             _subject.MessageGuid = null;
             _mockMessagingService.Setup(x => x.Connect(It.IsAny<string>(), It.IsAny<IMessageHandler>()))
                        .Callback<string, IMessageHandler>((p1, p2) => p2.MessageReceived(Guid.NewGuid().ToString()));
             InitializeMockClient();

             _subject.Initialize();

             _mockClippings.Verify(m => m.GetLastAsync(_subject));
         }

         [Test]
         public void PutDataAlwaysCallsApiSaveClippingAsync()
         {
             InitializeMockClient();

             _subject.PutData("data");

             _mockClippings.Verify(m => m.SaveAsync("data", _subject));
         }

         [Test]
         public void SaveClippingSucceededAlwaysCallsMessagingService()
         {
             InitializeMockClient();

             ((ISaveClippingCompleteHandler)_subject).SaveClippingSucceeded();

             _mockMessagingService.Verify(m => m.SendAsync(It.IsAny<string>(), _subject.MessageGuid, It.IsAny<IMessageHandler>()));
         }

         [Test]
         public void SaveClippingSucceededAlwaysSetsAnotherGuid()
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
