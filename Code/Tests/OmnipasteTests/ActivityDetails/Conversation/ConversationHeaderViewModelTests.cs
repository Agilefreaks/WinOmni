namespace OmnipasteTests.ActivityDetails.Conversation
{
    using System;
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

    [TestFixture]
    public class ConversationHeaderViewModelTests
    {
        private ConversationHeaderViewModel _subject;

        private Mock<IDevices> _mockDevices;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;

            _mockDevices = new Mock<IDevices> { DefaultValue = DefaultValue.Mock };

            _subject = new ConversationHeaderViewModel { Devices = _mockDevices.Object };
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
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
            _subject.Model = new ActivityPresenter(new Activity(@event));

            _subject.Call();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            _mockDevices.Verify(m => m.Call(phoneNumber));
        }

        [Test]
        public void Call_WhenCanceled_DoesNotInitiateACall()
        {
            var phoneNumber = "1234567890";
            var @event = new Event { PhoneNumber = phoneNumber };
            _subject.Model = new ActivityPresenter(new Activity(@event));

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
            _subject.Model = new ActivityPresenter(new Activity(@event));

            _subject.Call();
            _subject.CancelCall();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            _subject.State.Should().Be(ConversationHeaderStateEnum.Normal);
        }

        [Test]
        public void Call_OnCallInitiated_ChangesStateToCalling()
        {
            var @event = new Event { PhoneNumber = "1234567890" };
            _subject.Model = new ActivityPresenter(new Activity(@event));
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
            _subject.Model = new ActivityPresenter(new Activity(@event));
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
        public void UndoDelete_Always_ChangesStateToNormal()
        {
            _subject.UndoDelete();

            _subject.State.Should().Be(ConversationHeaderStateEnum.Normal);
        }
    }
}
