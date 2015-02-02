namespace OmnipasteTests.ActivityDetails.Conversation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails.Conversation;
    using Omnipaste.WorkspaceDetails.Conversation.Call;
    using Omnipaste.WorkspaceDetails.Conversation.Message;

    [TestFixture]
    public class ConversationContentViewModelTests
    {
        private ConversationContentViewModel _subject;

        private Mock<IMessageRepository> _mockMessageRepository;

        private Mock<ICallRepository> _mockCallRepository;

        private Mock<IEventAggregator> _mockEventAggregator;

        private TestScheduler _testScheduler;

        [SetUp]
        public void Setup()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;
            var mockingKernel = new MoqMockingKernel();
            mockingKernel.Bind<ICallViewModel>().ToMethod(context => CreateMock<ICallViewModel>());
            mockingKernel.Bind<IMessageViewModel>().ToMethod(context => CreateMock<IMessageViewModel>());
            _mockMessageRepository = new Mock<IMessageRepository> { DefaultValue = DefaultValue.Mock };
            _mockCallRepository = new Mock<ICallRepository> { DefaultValue = DefaultValue.Mock };
            _mockEventAggregator = new Mock<IEventAggregator> { DefaultValue = DefaultValue.Mock };
            _subject = new ConversationContentViewModel
                           {
                               Kernel = mockingKernel,
                               MessageRepository = _mockMessageRepository.Object,
                               CallRepository = _mockCallRepository.Object,
                               EventAggregator = _mockEventAggregator.Object
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
            _subject.Model = new ContactInfoPresenter(contactInfo);
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

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            _subject.Items.Count().Should().Be(2);
        }

        [Test]
        public void Activate_WhenCallWasNotViewed_MarksCallAsViewed()
        {
            var contactInfo = new ContactInfo();
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var call = new Call { UniqueId = "42" };
            var calls = new[] { call };
            var messageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Message>>>(100, Notification.CreateOnNext(Enumerable.Empty<Message>())));
            var callObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Call>>>(100, Notification.CreateOnNext(new List<Call>(calls).AsEnumerable())));
            _mockMessageRepository.Setup(x => x.GetAll(It.IsAny<Func<Message, bool>>())).Returns(messageObservable);
            _mockCallRepository.Setup(x => x.GetAll(It.IsAny<Func<Call, bool>>())).Returns(callObservable);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            call.WasViewed.Should().BeTrue();
        }

        [Test]
        public void Activate_WhenCallWasNotViewed_DismissesNotificationForCall()
        {
            var contactInfo = new ContactInfo();
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var call = new Call { UniqueId = "42" };
            var calls = new[] { call };
            var messageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Message>>>(100, Notification.CreateOnNext(Enumerable.Empty<Message>())));
            var callObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Call>>>(100, Notification.CreateOnNext(new List<Call>(calls).AsEnumerable())));
            _mockMessageRepository.Setup(x => x.GetAll(It.IsAny<Func<Message, bool>>())).Returns(messageObservable);
            _mockCallRepository.Setup(x => x.GetAll(It.IsAny<Func<Call, bool>>())).Returns(callObservable);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            _mockEventAggregator.Verify(m => m.Publish(It.Is<DismissNotification>(n => n.Identifier == call.UniqueId), It.IsAny<Action<Action>>()));
        }

        [Test]
        public void Activate_WhenCallWasNotViewed_SavesCall()
        {
            var contactInfo = new ContactInfo();
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var call = new Call { UniqueId = "42" };
            var calls = new[] { call };
            var messageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Message>>>(100, Notification.CreateOnNext(Enumerable.Empty<Message>())));
            var callObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Call>>>(100, Notification.CreateOnNext(new List<Call>(calls).AsEnumerable())));
            _mockMessageRepository.Setup(x => x.GetAll(It.IsAny<Func<Message, bool>>())).Returns(messageObservable);
            _mockCallRepository.Setup(x => x.GetAll(It.IsAny<Func<Call, bool>>())).Returns(callObservable);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            _mockCallRepository.Verify(m => m.Save(call));
        }

        [Test]
        public void ACallAppearsInTheCallStore_TheCallContactInfoHasTheSamePhoneNumberAsTheCurrentContactInfo_AddsACallViewModel()
        {
            var callFromContact = new Call { ContactInfo = new ContactInfo { Phone = "123" } };
            _subject.Model = new ContactInfoPresenter(new ContactInfo { Phone = "123" });
            var callObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<RepositoryOperation<Call>>>(100, Notification.CreateOnNext(new RepositoryOperation<Call>(RepositoryMethodEnum.Create, new Call()))),
                new Recorded<Notification<RepositoryOperation<Call>>>(200, Notification.CreateOnNext(new RepositoryOperation<Call>(RepositoryMethodEnum.Create, callFromContact))));
            _mockCallRepository.Setup(x => x.OperationObservable).Returns(callObservable);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            var children = _subject.Items.ToList();
            children.Count().Should().Be(1);
        }

        [Test]
        public void OnActivate_Always_AddsAMessageViewModelForEachMessageInTheStoreForTheCurrentContactInfo()
        {
            var contactInfo = new ContactInfo();
            _subject.Model = new ContactInfoPresenter(contactInfo);
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

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            _subject.Items.Count().Should().Be(2);
        }

        [Test]
        public void AMessageAppearsInTheMessageStore_TheMessageContactInfoHasTheSamePhoneNumberAsTheCurrentContactInfo_AddsAMessageViewModel()
        {
            var messageFromContact = new Message { ContactInfo = new ContactInfo { Phone = "123" } };
            _subject.Model = new ContactInfoPresenter(new ContactInfo { Phone = "123" });
            var messageObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<RepositoryOperation<Message>>>(100, Notification.CreateOnNext(new RepositoryOperation<Message>(RepositoryMethodEnum.Create, new Message()))),
                new Recorded<Notification<RepositoryOperation<Message>>>(200, Notification.CreateOnNext(new RepositoryOperation<Message>(RepositoryMethodEnum.Create, messageFromContact))));
            _mockMessageRepository.Setup(x => x.OperationObservable).Returns(messageObservable);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            var children = _subject.Items.ToList();
            children.Count().Should().Be(1);
        }

        [Test]
        public void OnActivate_Always_OrdersItemsForMessagesAndCallsAccordingToTheirTime()
        {
            var contactInfo = new ContactInfo();
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var baseTime = DateTime.Now;
            var call1 = new Call { Time = baseTime };
            var call2 = new Call { Time = baseTime.Add(TimeSpan.FromSeconds(10)) };
            _mockCallRepository.Setup(x => x.GetAll(It.IsAny<Func<Call, bool>>())).Returns(Observable.Return(new List<Call> { call1, call2 }));
            var message1 = new Message { Time = baseTime.Add(TimeSpan.FromSeconds(5)) };
            var message2 = new Message { Time = baseTime.Add(TimeSpan.FromSeconds(15)) };
            _mockMessageRepository.Setup(x => x.GetAll(It.IsAny<Func<Message, bool>>()))
                .Returns(Observable.Return(new List<Message> { message1, message2 }));

            ((IActivate)_subject).Activate();

            _subject.Items.Count().Should().Be(4);
        }

        [Test]
        public void OnActivate_PreviousActivationOccured_DoesNotAddViewModelsMultipleTimes()
        {
            var contactInfo = new ContactInfo();
            _subject.Model = new ContactInfoPresenter(contactInfo);
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

            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceBy(150);
            ((IDeactivate)_subject).Deactivate(false);
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceBy(150);

            _subject.Items.Count().Should().Be(2);
        }

        private static TViewModel CreateMock<TViewModel>()
            where TViewModel : class
        {
            var mock = new Mock<TViewModel> { DefaultValue = DefaultValue.Mock };
            mock.SetupAllProperties();
            return mock.Object;
        }
    }
}