namespace OmnipasteTests.ActivityDetails.Conversation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using Events.Models;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using Omnipaste.ActivityDetails.Conversation;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;
    using OmniUI.Models;
    using OmniUI.Presenters;

    [TestFixture]
    public class ConversationHeaderViewModelTests
    {
        private ConversationHeaderViewModel _subject;

        private Mock<IDevices> _mockDevices;

        private Mock<ICallRepository> _mockCallRepository;

        private Mock<IMessageRepository> _mockMessageRepository;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;

            _mockDevices = new Mock<IDevices> { DefaultValue = DefaultValue.Mock };
            _mockCallRepository = new Mock<ICallRepository> { DefaultValue = DefaultValue.Mock };
            _mockMessageRepository = new Mock<IMessageRepository> { DefaultValue = DefaultValue.Mock };

            _subject = new ConversationHeaderViewModel
                           {
                               Devices = _mockDevices.Object,
                               CallRepository = _mockCallRepository.Object,
                               MessageRepository = _mockMessageRepository.Object,
                               ContactInfo = new ContactInfoPresenter(new ContactInfo())
                           };
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;
        }

        [Test]
        public void Ctor_Always_SetsStateToNormal()
        {
            _subject = new ConversationHeaderViewModel();

            _subject.State.Should().Be(ConversationHeaderStateEnum.Normal);
        }

        [Test]
        public void Call_Always_InitiatesACall()
        {
            var phoneNumber = "1234567890";
            var @event = new Event { PhoneNumber = phoneNumber };
            _subject.Model = new ActivityPresenter(new Call(@event));

            _subject.Call();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            _mockDevices.Verify(m => m.Call(phoneNumber));
        }

        [Test]
        public void Call_WhenCanceled_DoesNotInitiateACall()
        {
            var phoneNumber = "1234567890";
            var @event = new Event { PhoneNumber = phoneNumber };
            _subject.Model = new ActivityPresenter(new Call(@event));

            _subject.Call();
            _subject.CancelCall();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            _mockDevices.Verify(m => m.Call(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Call_WhenCanceled_ChangesStateToNormal()
        {
            var phoneNumber = "1234567890";
            var @event = new Event { PhoneNumber = phoneNumber };
            _subject.Model = new ActivityPresenter(new Call(@event));

            _subject.Call();
            _subject.CancelCall();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            _subject.State.Should().Be(ConversationHeaderStateEnum.Normal);
        }

        [Test]
        public void Call_OnCallInitiated_ChangesStateToCalling()
        {
            var @event = new Event { PhoneNumber = "1234567890" };
            _subject.Model = new ActivityPresenter(new Call(@event));
            var callObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<EmptyModel>>(100, Notification.CreateOnNext(new EmptyModel())),
                new Recorded<Notification<EmptyModel>>(200, Notification.CreateOnCompleted<EmptyModel>()));
            _mockDevices.Setup(m => m.Call(It.IsAny<string>())).Returns(callObservable);
            
            _subject.Call();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            _subject.State.Should().Be(ConversationHeaderStateEnum.Calling);
        }

        [Test]
        public void Call_OnInitiated_ChangesStateToCalling()
        {
            var @event = new Event { PhoneNumber = "1234567890" };
            _subject.Model = new ActivityPresenter(new Call(@event));
            var callObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<EmptyModel>>(100, Notification.CreateOnNext(new EmptyModel())),
                new Recorded<Notification<EmptyModel>>(200, Notification.CreateOnCompleted<EmptyModel>()));
            _mockDevices.Setup(m => m.Call(It.IsAny<string>())).Returns(callObservable);
            
            _subject.Call();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10).Ticks);

            _subject.State.Should().Be(ConversationHeaderStateEnum.Normal);
        }

        [Test]
        public void Delete_Always_ChangesStateToDeleted()
        {
            _subject.Delete();

            _subject.State.Should().Be(ConversationHeaderStateEnum.Deleted);
        }

        [Test]
        public void Delete_WhenCallsExistForContact_UpdatesIsDeletedForCall()
        {
            var call = new Call { UniqueId = "42" };
            var getCallObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Call>>>(100, Notification.CreateOnNext(new List<Call> { call }.AsEnumerable())));
            _mockCallRepository.Setup(m => m.GetAll(It.IsAny<Func<Call, bool>>())).Returns(getCallObservable);

            _subject.Delete();
            _testScheduler.AdvanceBy(1000);

            _mockCallRepository.Verify(m => m.Save(call));
            call.IsDeleted.Should().BeTrue();
        }

        [Test]
        public void Delete_WhenMessagesExistForContact_DeletesEachMessage()
        {
            var message = new Message { UniqueId = "42" };
            var getMessageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Message>>>(100, Notification.CreateOnNext(new List<Message> { message }.AsEnumerable())));
            _mockMessageRepository.Setup(m => m.GetAll(It.IsAny<Func<Message, bool>>())).Returns(getMessageObservable);

            _subject.Delete();
            _testScheduler.AdvanceBy(1000);

            _mockMessageRepository.Verify(m => m.Save(message));
            message.IsDeleted.Should().BeTrue();
        }

        [Test]
        public void UndoDelete_Always_ChangesStateToNormal()
        {
            _subject.UndoDelete();

            _subject.State.Should().Be(ConversationHeaderStateEnum.Normal);
        }

        [Test]
        public void UndoDelete_WhenCallsWereDeleted_RestoresCalls()
        {
            var call = new Call { UniqueId = "42", IsDeleted = true };
            var getCallObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Call>>>(100, Notification.CreateOnNext(new List<Call> { call }.AsEnumerable())));
            _mockCallRepository.Setup(m => m.GetAll(It.IsAny<Func<Call, bool>>())).Returns(getCallObservable);
            
            _subject.UndoDelete();
            _testScheduler.AdvanceBy(1000);

            _mockCallRepository.Verify(m => m.Save(call));
            call.IsDeleted.Should().BeFalse();
        }

        [Test]
        public void UndoDelete_WhenMessagesWereDeleted_RestoresMessages()
        {
            var message = new Message { UniqueId = "42", IsDeleted = true };
            var getMessageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Message>>>(100, Notification.CreateOnNext(new List<Message> { message }.AsEnumerable())));
            _mockMessageRepository.Setup(m => m.GetAll(It.IsAny<Func<Message, bool>>())).Returns(getMessageObservable);

            _subject.UndoDelete();
            _testScheduler.AdvanceBy(1000);

            _mockMessageRepository.Verify(m => m.Save(message));
            message.IsDeleted.Should().BeFalse();
        }
    }
}
