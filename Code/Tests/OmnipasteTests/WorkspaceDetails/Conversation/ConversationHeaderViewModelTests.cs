namespace OmnipasteTests.WorkspaceDetails.Conversation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reactive;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Entities;
    using Omnipaste.Factories;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails.Conversation;
    using OmnipasteTests.Helpers;
    using PhoneCalls.Dto;
    using PhoneCalls.Resources.v1;

    [TestFixture]
    public class ConversationHeaderViewModelTests
    {
        private ConversationHeaderViewModel _subject;

        private Mock<IPhoneCalls> _mockPhoneCalls;

        private Mock<IPhoneCallRepository> _mockPhoneCallRepository;

        private Mock<IPhoneCallFactory> _mockPhoneCallFactory;

        private Mock<ISmsMessageRepository> _mockMessageRepository;

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
            _mockMessageRepository = new Mock<ISmsMessageRepository> { DefaultValue = DefaultValue.Mock };

            _subject = new ConversationHeaderViewModel
                           {
                               PhoneCalls = _mockPhoneCalls.Object,
                               PhoneCallRepository = _mockPhoneCallRepository.Object,
                               PhoneCallFactory = _mockPhoneCallFactory.Object,
                               SmsMessageRepository = _mockMessageRepository.Object,
                               Model = new ContactModel(new ContactEntity())
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
        public void OnRecepients_WhenMoreThanTwo_WillChangeState()
        {
            _subject.Recipients = new ObservableCollection<ContactModel>
                                      {
                                          new ContactModel(new ContactEntity()),
                                          new ContactModel(new ContactEntity())
                                      };


            _subject.State.Should().Be(ConversationHeaderStateEnum.Group);
        }

        [Test]
        public void OnRecepientAdded_WhenMoreThanTwo_WillChangeState()
        {
            _subject.Recipients = new ObservableCollection<ContactModel>();

            ((IActivate)_subject).Activate();
            _subject.Recipients.Add(new ContactModel(new ContactEntity()));
            _subject.Recipients.Add(new ContactModel(new ContactEntity()));

            _subject.State.Should().Be(ConversationHeaderStateEnum.Group);
        }

        [Test]
        public void Call_Always_InitiatesACall()
        {
            const string PhoneNumber = "1234567890";
            var contactEntity = new ContactEntity
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
            _subject.Model = new ContactModel(contactEntity);

            _subject.Call();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            _mockPhoneCalls.Verify(m => m.Call(PhoneNumber, It.IsAny<int?>()));
        }

        [Test]
        public void Call_WhenCanceled_DoesNotInitiateACall()
        {
            const string PhoneNumber = "1234567890";
            var contactEntity = new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = PhoneNumber } } };
            _subject.Model = new ContactModel(contactEntity);

            _subject.Call();
            _subject.CancelCall();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            _mockPhoneCalls.Verify(m => m.Call(It.IsAny<string>(), It.IsAny<int?>()), Times.Never());
        }

        [Test]
        public void Call_WhenCanceled_ChangesStateToNormal()
        {
            const string PhoneNumber = "1234567890";
            var contactEntity = new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = PhoneNumber } } };
            _subject.Model = new ContactModel(contactEntity);

            _subject.Call();
            _subject.CancelCall();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            _subject.State.Should().Be(ConversationHeaderStateEnum.Normal);
        }

        [Test]
        public void Call_OnCallInitiated_ChangesStateToCalling()
        {
            var contactEntity = new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "1234567890" } } };
            _subject.Model = new ContactModel(contactEntity);
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
            var contactEntity = new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "1234567890" } } };
            _subject.Model = new ContactModel(contactEntity);
            var callObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<PhoneCallDto>>(100, Notification.CreateOnNext(new PhoneCallDto())),
                new Recorded<Notification<PhoneCallDto>>(200, Notification.CreateOnCompleted<PhoneCallDto>()));
            var phoneFactoryObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<LocalPhoneCallEntity>>(110, Notification.CreateOnNext(new LocalPhoneCallEntity())),
                new Recorded<Notification<LocalPhoneCallEntity>>(200, Notification.CreateOnCompleted<LocalPhoneCallEntity>()));
            _mockPhoneCalls.Setup(m => m.Call(It.IsAny<string>(), It.IsAny<int?>())).Returns(callObservable);
            _mockPhoneCallFactory.Setup(m => m.Create<LocalPhoneCallEntity>(It.IsAny<PhoneCallDto>())).Returns(phoneFactoryObservable);
            SetupSaveCallObservable();

            _subject.Call();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10).Ticks);

            _subject.State.Should().Be(ConversationHeaderStateEnum.Normal);
        }

        [Test]
        public void Call_AfterCreatingTheCall_CallsPhoneFactoryCreate()
        {
            var contactEntity = new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "1234567890" } } };
            _subject.Model = new ContactModel(contactEntity);
            var callObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<PhoneCallDto>>(100, Notification.CreateOnNext(new PhoneCallDto())),
                new Recorded<Notification<PhoneCallDto>>(200, Notification.CreateOnCompleted<PhoneCallDto>()));
            _mockPhoneCalls.Setup(m => m.Call(It.IsAny<string>(), It.IsAny<int?>())).Returns(callObservable);
            SetupSaveCallObservable();

            _subject.Call();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10).Ticks);

            _mockPhoneCallFactory.Verify(x => x.Create<LocalPhoneCallEntity>(It.IsAny<PhoneCallDto>()), Times.Once());
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
            var call = new LocalPhoneCallEntity { UniqueId = "42" };
            var getCallObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<PhoneCallEntity>>>(100, Notification.CreateOnNext(new List<PhoneCallEntity> { call }.AsEnumerable())));
            _mockPhoneCallRepository.Setup(m => m.GetConversationForContact(It.IsAny<ContactEntity>())).Returns(getCallObservable);

            _subject.Delete();
            _testScheduler.AdvanceBy(1000);

            call.IsDeleted.Should().BeTrue();
        }

        [Test]
        public void Delete_WhenMessagesExistForContact_DeletesEachMessage()
        {
            var message = new TestSmsMessageEntity { UniqueId = "42" };
            var getMessageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<SmsMessageEntity>>>(100, Notification.CreateOnNext(new List<SmsMessageEntity> { message }.AsEnumerable())));
            _mockMessageRepository.Setup(m => m.GetConversationForContact(It.IsAny<ContactEntity>())).Returns(getMessageObservable);

            _subject.Delete();
            _testScheduler.AdvanceBy(1000);

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
            var call = new LocalPhoneCallEntity { UniqueId = "42", IsDeleted = true };
            var getCallObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<PhoneCallEntity>>>(100, Notification.CreateOnNext(new List<PhoneCallEntity> { call }.AsEnumerable())));
            _mockPhoneCallRepository.Setup(m => m.GetConversationForContact(It.IsAny<ContactEntity>())).Returns(getCallObservable);
            
            _subject.UndoDelete();
            _testScheduler.AdvanceBy(1000);

            call.IsDeleted.Should().BeFalse();
        }

        [Test]
        public void UndoDelete_WhenMessagesWereDeleted_RestoresMessages()
        {
            var message = new TestSmsMessageEntity { UniqueId = "42", IsDeleted = true };
            var getMessageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<SmsMessageEntity>>>(100, Notification.CreateOnNext(new List<SmsMessageEntity> { message }.AsEnumerable())));
            _mockMessageRepository.Setup(m => m.GetConversationForContact(It.IsAny<ContactEntity>())).Returns(getMessageObservable);

            _subject.UndoDelete();
            _testScheduler.AdvanceBy(1000);

            message.IsDeleted.Should().BeFalse();
        }

        private void SetupSaveCallObservable()
        {
            var saveObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<PhoneCallEntity>>>(
                        100,
                        Notification.CreateOnNext(new RepositoryOperation<PhoneCallEntity>(RepositoryMethodEnum.Changed, new LocalPhoneCallEntity()))),
                    new Recorded<Notification<RepositoryOperation<PhoneCallEntity>>>(
                        200,
                        Notification.CreateOnCompleted<RepositoryOperation<PhoneCallEntity>>()));
            _mockPhoneCallRepository.Setup(x => x.Save(It.IsAny<PhoneCallEntity>())).Returns(saveObservable);
        }
    }
}
