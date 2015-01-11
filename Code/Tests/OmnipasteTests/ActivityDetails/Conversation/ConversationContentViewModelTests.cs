namespace OmnipasteTests.ActivityDetails.Conversation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.ActivityDetails.Conversation;
    using Omnipaste.ActivityDetails.Conversation.Call;
    using Omnipaste.ActivityDetails.Conversation.Message;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using OmniUI.Details;
    using OmniUI.Models;

    [TestFixture]
    public class ConversationContentViewModelTests
    {
        private IConversationContentViewModel _subject;

        private Mock<IMessageRepository> _mockMessageRepository;

        private Mock<ICallRepository> _mockCallRepository;

        private TestScheduler _testScheduler;

        [SetUp]
        public void Setup()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;

            var mockingKernel = new MoqMockingKernel();
            mockingKernel.Bind<ICallViewModel>().ToMethod(context => GetMockDetailsViewModel<ICallViewModel, Call>());
            mockingKernel.Bind<IMessageViewModel>()
                .ToMethod(context => GetMockDetailsViewModel<IMessageViewModel, Message>());
            _mockMessageRepository = new Mock<IMessageRepository> { DefaultValue = DefaultValue.Mock };
            _mockCallRepository = new Mock<ICallRepository> { DefaultValue = DefaultValue.Mock };
            _subject = new ConversationContentViewModel
                           {
                               Kernel = mockingKernel,
                               MessageRepository = _mockMessageRepository.Object,
                               CallRepository = _mockCallRepository.Object
                           };
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void OnActivate_Always_AddsACallViewModelForEachCallInTheStoreForTheCurrentContactInfo()
        {
            var contactInfo = new ContactInfo();
            _subject.ContactInfo = contactInfo;
            var call1 = new Call();
            var call2 = new Call();
            var calls = new[] { call1, call2 };
            var messageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Message>>>(100, Notification.CreateOnNext(Enumerable.Empty<Message>())));
            var callObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Call>>>(100, Notification.CreateOnNext(new List<Call>(calls).AsEnumerable())));
            _mockMessageRepository.Setup(x => x.GetAll(It.IsAny<Func<Message, bool>>())).Returns(messageObservable);
            _mockCallRepository.Setup(x => x.GetAll(It.IsAny<Func<Call, bool>>())).Returns(callObservable);

            _subject.Activate();
            _testScheduler.Start();

            var children = _subject.GetChildren();
            calls.All(call => children.Any(child => ((IDetailsViewModel)child).Model == call)).Should().BeTrue();
        }

        [Test]
        public void ACallAppearsInTheCallStore_TheCallContactInfoHasTheSamePhoneNumberAsTheCurrentContactInfo_AddsACallViewModel()
        {
            var callFromContact = new Call { ContactInfo = new ContactInfo { Phone = "123" } };
            _subject.ContactInfo = new ContactInfo { Phone = "123" };
            var callObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<RepositoryOperation<Call>>>(100, Notification.CreateOnNext(new RepositoryOperation<Call>(RepositoryMethodEnum.Save, new Call()))),
                new Recorded<Notification<RepositoryOperation<Call>>>(200, Notification.CreateOnNext(new RepositoryOperation<Call>(RepositoryMethodEnum.Save, callFromContact))));
            _mockCallRepository.Setup(x => x.OperationObservable).Returns(callObservable);

            _subject.Activate();
            _testScheduler.Start();

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
            var messageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Message>>>(100, Notification.CreateOnNext(new List<Message>(messages).AsEnumerable())));
            var callObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Call>>>(100, Notification.CreateOnNext(Enumerable.Empty<Call>())));
            _mockMessageRepository.Setup(x => x.GetAll(It.IsAny<Func<Message, bool>>())).Returns(messageObservable);
            _mockCallRepository.Setup(x => x.GetAll(It.IsAny<Func<Call, bool>>())).Returns(callObservable);

            _subject.Activate();
            _testScheduler.Start();

            var children = _subject.GetChildren();
            messages.All(call => children.Any(child => ((IDetailsViewModel)child).Model == call)).Should().BeTrue();
        }

        [Test]
        public void AMessageAppearsInTheMessageStore_TheMessageContactInfoHasTheSamePhoneNumberAsTheCurrentContactInfo_AddsAMessageViewModel()
        {
            var messageFromContact = new Message { ContactInfo = new ContactInfo { Phone = "123" } };
            _subject.ContactInfo = new ContactInfo { Phone = "123" };
            var messageObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<RepositoryOperation<Message>>>(100, Notification.CreateOnNext(new RepositoryOperation<Message>(RepositoryMethodEnum.Save, new Message()))),
                new Recorded<Notification<RepositoryOperation<Message>>>(200, Notification.CreateOnNext(new RepositoryOperation<Message>(RepositoryMethodEnum.Save, messageFromContact))));
            _mockMessageRepository.Setup(x => x.OperationObservable).Returns(messageObservable);

            _subject.Activate();
            _testScheduler.Start();

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
            _mockCallRepository.Setup(x => x.GetAll(It.IsAny<Func<Call, bool>>())).Returns(Observable.Return(new List<Call> { call1, call2 }));
            var message1 = new Message { Time = baseTime.Add(TimeSpan.FromSeconds(5)) };
            var message2 = new Message { Time = baseTime.Add(TimeSpan.FromSeconds(15)) };
            _mockMessageRepository.Setup(x => x.GetAll(It.IsAny<Func<Message, bool>>()))
                .Returns(Observable.Return(new List<Message> { message1, message2 }));

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
            var calls = new[] { call1, call2 };
            var callObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Call>>>(100, Notification.CreateOnNext(new List<Call>(calls).AsEnumerable())));
            var messageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Message>>>(100, Notification.CreateOnNext(Enumerable.Empty<Message>())));
            _mockCallRepository.Setup(x => x.GetAll(It.IsAny<Func<Call, bool>>())).Returns(callObservable);
            _mockMessageRepository.Setup(x => x.GetAll(It.IsAny<Func<Message, bool>>())).Returns(messageObservable);

            _testScheduler.Start();
            _subject.Activate();
            _testScheduler.AdvanceBy(150);
            _subject.Deactivate(false);
            _subject.Activate();
            _testScheduler.AdvanceBy(150);

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