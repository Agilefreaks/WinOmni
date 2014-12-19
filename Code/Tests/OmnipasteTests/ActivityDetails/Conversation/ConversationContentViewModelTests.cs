namespace OmnipasteTests.ActivityDetails.Conversation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.ActivityDetails.Conversation;
    using Omnipaste.ActivityDetails.Conversation.Call;
    using Omnipaste.ActivityDetails.Conversation.Message;
    using Omnipaste.Models;
    using Omnipaste.Services;
    using OmniUI.Details;

    [TestFixture]
    public class ConversationContentViewModelTests
    {
        private IConversationContentViewModel _subject;

        private Mock<IMessageStore> _mockMessageStore;

        private Mock<ICallStore> _mockCallStore;

        [SetUp]
        public void Setup()
        {
            var mockingKernel = new MoqMockingKernel();
            mockingKernel.Bind<ICallViewModel>().ToMethod(context => GetMockDetailsViewModel<ICallViewModel, Call>());
            mockingKernel.Bind<IMessageViewModel>()
                .ToMethod(context => GetMockDetailsViewModel<IMessageViewModel, Message>());
            _mockMessageStore = new Mock<IMessageStore> { DefaultValue = DefaultValue.Mock };
            _mockCallStore = new Mock<ICallStore> { DefaultValue = DefaultValue.Mock };
            _subject = new ConversationContentViewModel
                           {
                               Kernel = mockingKernel,
                               MessageStore = _mockMessageStore.Object,
                               CallStore = _mockCallStore.Object
                           };
        }

        [Test]
        public void OnActivate_Always_AddsACallViewModelForEachCallInTheStoreForTheCurrentContactInfo()
        {
            var contactInfo = new ContactInfo();
            _subject.ContactInfo = contactInfo;
            var call1 = new Call();
            var call2 = new Call();
            var calls = new [] { call1, call2 };
            _mockCallStore.Setup(x => x.GetRelatedCalls(contactInfo)).Returns(new List<Call>(calls));

            _subject.Activate();

            var children = _subject.GetChildren();
            calls.All(call => children.Any(child => ((IDetailsViewModel)child).Model == call)).Should().BeTrue();
        }

        [Test]
        public void ACallAppearsInTheCallStore_TheCallContactInfoHasTheSamePhoneNumberAsTheCurrentContactInfo_AddsACallViewModel()
        {
            var testScheduler = new TestScheduler();
            var callFromContact = new Call { ContactInfo = new ContactInfo { Phone = "123" } };
            _subject.ContactInfo = new ContactInfo { Phone = "123" };
            var callObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<Call>>(100, Notification.CreateOnNext(new Call())),
                new Recorded<Notification<Call>>(200, Notification.CreateOnNext(callFromContact)));
            _mockCallStore.Setup(x => x.CallObservable).Returns(callObservable);

            _subject.Activate();
            testScheduler.Start();

            var children = _subject.GetChildren().ToList();
            children.Count().Should().Be(1);
            ((ICallViewModel)children.First()).Model.Should().Be(callFromContact);
        }

        [Test]
        public void OnActivate_Always_AddsAMessageViewModelForEachMessageInTheStoreForTheCurrentContactInfo()
        {
            var contactInfo = new ContactInfo();
            _subject.ContactInfo = contactInfo;
            var message1 = new Message();
            var message2 = new Message();
            var messages = new [] { message1, message2 };
            _mockMessageStore.Setup(x => x.GetRelatedMessages(contactInfo)).Returns(new List<Message>(messages));

            _subject.Activate();

            var children = _subject.GetChildren();
            messages.All(call => children.Any(child => ((IDetailsViewModel)child).Model == call)).Should().BeTrue();
        }

        [Test]
        public void AMessageAppearsInTheMessageStore_TheMessageContactInfoHasTheSamePhoneNumberAsTheCurrentContactInfo_AddsAMessageViewModel()
        {
            var testScheduler = new TestScheduler();
            var messageFromContact = new Message { ContactInfo = new ContactInfo { Phone = "123" } };
            _subject.ContactInfo = new ContactInfo { Phone = "123" };
            var messageObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<Message>>(100, Notification.CreateOnNext(new Message())),
                new Recorded<Notification<Message>>(200, Notification.CreateOnNext(messageFromContact)));
            _mockMessageStore.Setup(x => x.MessageObservable).Returns(messageObservable);

            _subject.Activate();
            testScheduler.Start();

            var children = _subject.GetChildren().ToList();
            children.Count().Should().Be(1);
            ((IMessageViewModel)children.First()).Model.Should().Be(messageFromContact);
        }

        [Test]
        public void OnActivate_Always_OrdersItemsForMessagesAndCallsAccordingToTheirTime()
        {
            var contactInfo = new ContactInfo();
            _subject.ContactInfo = contactInfo;
            var baseTime = DateTime.Now;
            var call1 = new Call { Time = baseTime };
            var call2 = new Call { Time = baseTime.Add(TimeSpan.FromSeconds(10)) };
            _mockCallStore.Setup(x => x.GetRelatedCalls(contactInfo)).Returns(new List<Call> { call1, call2 });
            var message1 = new Message { Time = baseTime.Add(TimeSpan.FromSeconds(5)) };
            var message2 = new Message { Time = baseTime.Add(TimeSpan.FromSeconds(15)) };
            _mockMessageStore.Setup(x => x.GetRelatedMessages(contactInfo))
                .Returns(new List<Message> { message1, message2 });

            _subject.Activate();

            var screens = _subject.GetChildren().Cast<IDetailsViewModel>().ToList();
            screens.Count.Should().Be(4);
            screens[0].Model.Should().Be(call1);
            screens[1].Model.Should().Be(message1);
            screens[2].Model.Should().Be(call2);
            screens[3].Model.Should().Be(message2);
        }

        [Test]
        public void OnActivate_PreviousActivationOccured_DoesNotAddViewModelsMultipleTimes()
        {
            var contactInfo = new ContactInfo();
            _subject.ContactInfo = contactInfo;
            var call1 = new Call();
            var call2 = new Call();
            var calls = new [] { call1, call2 };
            _mockCallStore.Setup(x => x.GetRelatedCalls(contactInfo)).Returns(new List<Call>(calls));

            _subject.Activate();
            _subject.Deactivate(false);
            _subject.Activate();

            _subject.GetChildren().Count().Should().Be(2);
        }

        private static TViewModel GetMockDetailsViewModel<TViewModel, TModel>()
            where TViewModel : class, IDetailsViewModel<TModel>
        {
            var callViewModel = new Mock<TViewModel>();
            callViewModel.SetupAllProperties();
            callViewModel.As<IDetailsViewModel>().SetupGet(x => x.Model).Returns(() => callViewModel.Object.Model);
            return callViewModel.Object;
        }
    }
}