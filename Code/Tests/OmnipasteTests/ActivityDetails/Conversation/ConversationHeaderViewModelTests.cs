namespace OmnipasteTests.ActivityDetails.Conversation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Factories;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails.Conversation;
    using OmnipasteTests.Helpers;
    using PhoneCalls.Models;
    using PhoneCalls.Resources.v1;

    [TestFixture]
    public class ConversationHeaderViewModelTests
    {
        private ConversationHeaderViewModel _subject;

        private Mock<IPhoneCalls> _mockPhoneCalls;

        private Mock<IPhoneCallRepository> _mockPhoneCallRepository;

        private Mock<IPhoneCallFactory> _mockPhoneCallFactory;

        private Mock<IMessageRepository> _mockMessageRepository;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;

            _mockPhoneCalls = new Mock<IPhoneCalls> { DefaultValue = DefaultValue.Mock };
            _mockPhoneCallRepository = new Mock<IPhoneCallRepository> { DefaultValue = DefaultValue.Mock };
            _mockPhoneCallFactory = new Mock<IPhoneCallFactory> { DefaultValue = DefaultValue.Mock };
            _mockMessageRepository = new Mock<IMessageRepository> { DefaultValue = DefaultValue.Mock };

            _subject = new ConversationHeaderViewModel
                           {
                               PhoneCalls = _mockPhoneCalls.Object,
                               PhoneCallRepository = _mockPhoneCallRepository.Object,
                               PhoneCallFactory = _mockPhoneCallFactory.Object,
                               MessageRepository = _mockMessageRepository.Object,
                               Model = new ContactInfoPresenter(new ContactInfo())
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
            const string PhoneNumber = "1234567890";
            var contactInfo = new ContactInfo
                                  {
                                      PhoneNumbers =
                                          new List<PhoneNumber>
                                              {
                                                  new PhoneNumber
                                                      {
                                                          Number = PhoneNumber,
                                                          Type = "Work"
                                                      }
                                              }
                                  };
            _subject.Model = new ContactInfoPresenter(contactInfo);

            _subject.Call();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            _mockPhoneCalls.Verify(m => m.Call(PhoneNumber, It.IsAny<int?>()));
        }

        [Test]
        public void Call_WhenCanceled_DoesNotInitiateACall()
        {
            const string PhoneNumber = "1234567890";
            var contactInfo = new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = PhoneNumber } } };
            _subject.Model = new ContactInfoPresenter(contactInfo);

            _subject.Call();
            _subject.CancelCall();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            _mockPhoneCalls.Verify(m => m.Call(It.IsAny<string>(), It.IsAny<int?>()), Times.Never());
        }

        [Test]
        public void Call_WhenCanceled_ChangesStateToNormal()
        {
            const string PhoneNumber = "1234567890";
            var contactInfo = new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = PhoneNumber } } };
            _subject.Model = new ContactInfoPresenter(contactInfo);

            _subject.Call();
            _subject.CancelCall();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            _subject.State.Should().Be(ConversationHeaderStateEnum.Normal);
        }

        [Test]
        public void Call_OnCallInitiated_ChangesStateToCalling()
        {
            var contactInfo = new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "1234567890" } } };
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var callObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<PhoneCallDto>>(100, Notification.CreateOnNext(new PhoneCallDto())),
                new Recorded<Notification<PhoneCallDto>>(200, Notification.CreateOnCompleted<PhoneCallDto>()));
            _mockPhoneCalls.Setup(m => m.Call(It.IsAny<string>(), It.IsAny<int?>())).Returns(callObservable);
            
            _subject.Call();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            _subject.State.Should().Be(ConversationHeaderStateEnum.Calling);
        }

        [Test]
        public void Call_OnInitiated_ChangesStateToCalling()
        {
            var contactInfo = new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "1234567890" } } };
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var callObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<PhoneCallDto>>(100, Notification.CreateOnNext(new PhoneCallDto())),
                new Recorded<Notification<PhoneCallDto>>(200, Notification.CreateOnCompleted<PhoneCallDto>()));
            var phoneFactoryObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<LocalPhoneCall>>(110, Notification.CreateOnNext(new LocalPhoneCall())),
                new Recorded<Notification<LocalPhoneCall>>(200, Notification.CreateOnCompleted<LocalPhoneCall>()));
            _mockPhoneCalls.Setup(m => m.Call(It.IsAny<string>(), It.IsAny<int?>())).Returns(callObservable);
            _mockPhoneCallFactory.Setup(m => m.Create<LocalPhoneCall>(It.IsAny<PhoneCallDto>())).Returns(phoneFactoryObservable);
            SetupSaveCallObservable();

            _subject.Call();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10).Ticks);

            _subject.State.Should().Be(ConversationHeaderStateEnum.Normal);
        }

        [Test]
        public void Call_AfterCreatingTheCall_CallsPhoneFactoryCreate()
        {
            var contactInfo = new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "1234567890" } } };
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var callObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<PhoneCallDto>>(100, Notification.CreateOnNext(new PhoneCallDto())),
                new Recorded<Notification<PhoneCallDto>>(200, Notification.CreateOnCompleted<PhoneCallDto>()));
            _mockPhoneCalls.Setup(m => m.Call(It.IsAny<string>(), It.IsAny<int?>())).Returns(callObservable);
            SetupSaveCallObservable();

            _subject.Call();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10).Ticks);

            _mockPhoneCallFactory.Verify(x => x.Create<LocalPhoneCall>(It.IsAny<PhoneCallDto>()), Times.Once());
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
            var call = new LocalPhoneCall { UniqueId = "42" };
            var getCallObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<PhoneCall>>>(100, Notification.CreateOnNext(new List<PhoneCall> { call }.AsEnumerable())));
            _mockPhoneCallRepository.Setup(m => m.GetAll(It.IsAny<Func<PhoneCall, bool>>())).Returns(getCallObservable);

            _subject.Delete();
            _testScheduler.AdvanceBy(1000);

            _mockPhoneCallRepository.Verify(m => m.Save(call));
            call.IsDeleted.Should().BeTrue();
        }

        [Test]
        public void Delete_WhenMessagesExistForContact_DeletesEachMessage()
        {
            var message = new TestSmsMessage { UniqueId = "42" };
            var getMessageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<SmsMessage>>>(100, Notification.CreateOnNext(new List<SmsMessage> { message }.AsEnumerable())));
            _mockMessageRepository.Setup(m => m.GetAll(It.IsAny<Func<SmsMessage, bool>>())).Returns(getMessageObservable);

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
            var call = new LocalPhoneCall { UniqueId = "42", IsDeleted = true };
            var getCallObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<PhoneCall>>>(100, Notification.CreateOnNext(new List<PhoneCall> { call }.AsEnumerable())));
            _mockPhoneCallRepository.Setup(m => m.GetAll(It.IsAny<Func<PhoneCall, bool>>())).Returns(getCallObservable);
            
            _subject.UndoDelete();
            _testScheduler.AdvanceBy(1000);

            _mockPhoneCallRepository.Verify(m => m.Save(call));
            call.IsDeleted.Should().BeFalse();
        }

        [Test]
        public void UndoDelete_WhenMessagesWereDeleted_RestoresMessages()
        {
            var message = new TestSmsMessage { UniqueId = "42", IsDeleted = true };
            var getMessageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<SmsMessage>>>(100, Notification.CreateOnNext(new List<SmsMessage> { message }.AsEnumerable())));
            _mockMessageRepository.Setup(m => m.GetAll(It.IsAny<Func<SmsMessage, bool>>())).Returns(getMessageObservable);

            _subject.UndoDelete();
            _testScheduler.AdvanceBy(1000);

            _mockMessageRepository.Verify(m => m.Save(message));
            message.IsDeleted.Should().BeFalse();
        }

        private void SetupSaveCallObservable()
        {
            var saveObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<PhoneCall>>>(
                        100,
                        Notification.CreateOnNext(new RepositoryOperation<PhoneCall>(RepositoryMethodEnum.Changed, new LocalPhoneCall()))),
                    new Recorded<Notification<RepositoryOperation<PhoneCall>>>(
                        200,
                        Notification.CreateOnCompleted<RepositoryOperation<PhoneCall>>()));
            _mockPhoneCallRepository.Setup(x => x.Save(It.IsAny<PhoneCall>())).Returns(saveObservable);
        }
    }
}
