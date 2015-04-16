namespace OmnipasteTests.NotificationList
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Clipboard.Dto;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.EventAggregatorMessages;
    using Omnipaste.Framework.Services.Repositories;
    using Omnipaste.Notifications.NotificationList;
    using Omnipaste.Notifications.NotificationList.Notification;
    using Omnipaste.Notifications.NotificationList.Notification.ClippingNotification;
    using Omnipaste.Notifications.NotificationList.Notification.IncomingCallNotification;
    using Omnipaste.Notifications.NotificationList.Notification.IncomingSmsNotification;

    [TestFixture]
    public class NotificationListViewModelTests
    {
        #region Methods

        private void SetupTestScheduler()
        {
            _testScheduler = new TestScheduler();
            _incommingClipping = new ClippingEntity { Source = ClippingDto.ClippingSourceEnum.Cloud };
            _viewedClipping = new ClippingEntity { Source = ClippingDto.ClippingSourceEnum.Cloud, WasViewed = true };
            _testableClippingsObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(
                        200,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingEntity>(RepositoryMethodEnum.Changed, _incommingClipping))),
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(
                        250,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingEntity>(
                                RepositoryMethodEnum.Changed,
                                new ClippingEntity { Source = ClippingDto.ClippingSourceEnum.Local }))),
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(
                        300,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingEntity>(RepositoryMethodEnum.Changed, _viewedClipping))));

            _incommingPhoneCallEntity = new RemotePhoneCallEntity();
            _viewedPhoneCallEntity = new RemotePhoneCallEntity { WasViewed = true };
            _testableCallsObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<PhoneCallEntity>>>(
                        200,
                        Notification.CreateOnNext(
                            new RepositoryOperation<PhoneCallEntity>(RepositoryMethodEnum.Changed, _incommingPhoneCallEntity))),
                    new Recorded<Notification<RepositoryOperation<PhoneCallEntity>>>(
                        250,
                        Notification.CreateOnNext(
                            new RepositoryOperation<PhoneCallEntity>(RepositoryMethodEnum.Changed, _viewedPhoneCallEntity))));
            var remoteSmsMessage = new RemoteSmsMessageEntity();
            var viewedSmsMessage = new RemoteSmsMessageEntity { WasViewed = true };
            _testableMessagesObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<SmsMessageEntity>>>(
                        200,
                        Notification.CreateOnNext(
                            new RepositoryOperation<SmsMessageEntity>(RepositoryMethodEnum.Changed, remoteSmsMessage))),
                    new Recorded<Notification<RepositoryOperation<SmsMessageEntity>>>(
                        250,
                        Notification.CreateOnNext(
                            new RepositoryOperation<SmsMessageEntity>(RepositoryMethodEnum.Changed, viewedSmsMessage))));
            SchedulerProvider.Dispatcher = _testScheduler;
        }

        #endregion

        private Mock<IClippingRepository> _mockClippingRepository;

        private Mock<IPhoneCallRepository> _mockCallRepository;

        private Mock<ISmsMessageRepository> _mockMessageRepository;

        private MoqMockingKernel _mockingKernel;

        private INotificationListViewModel _subject;

        private TestScheduler _testScheduler;

        private ITestableObservable<RepositoryOperation<ClippingEntity>> _testableClippingsObservable;

        private ITestableObservable<RepositoryOperation<PhoneCallEntity>> _testableCallsObservable;

        private ITestableObservable<RepositoryOperation<SmsMessageEntity>> _testableMessagesObservable;

        private Mock<INotificationViewModelFactory> _mockNotificationViewModelFactory;

        private Mock<IApplicationService> _mockApplicationService;

        private Mock<IEventAggregator> _mockEventAggregator;

        private PhoneCallEntity _incommingPhoneCallEntity;

        private PhoneCallEntity _viewedPhoneCallEntity;

        private ClippingEntity _incommingClipping;

        private ClippingEntity _viewedClipping;

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MoqMockingKernel();

            _mockingKernel.Bind<IDisposable>().ToMock();

            SetupTestScheduler();

            _mockClippingRepository = new Mock<IClippingRepository> { DefaultValue = DefaultValue.Mock };
            _mockingKernel.Bind<IClippingRepository>().ToConstant(_mockClippingRepository.Object);
            _mockCallRepository = new Mock<IPhoneCallRepository> { DefaultValue = DefaultValue.Mock };
            _mockingKernel.Bind<IPhoneCallRepository>().ToConstant(_mockCallRepository.Object);
            _mockMessageRepository = new Mock<ISmsMessageRepository> { DefaultValue = DefaultValue.Mock };
            _mockingKernel.Bind<ISmsMessageRepository>().ToConstant(_mockMessageRepository.Object);
            _mockingKernel.Bind<INotificationListViewModel>().To<NotificationListViewModel>();
            _mockApplicationService = _mockingKernel.GetMock<IApplicationService>();
            _mockingKernel.Bind<IApplicationService>().ToConstant(_mockApplicationService.Object);
            _mockNotificationViewModelFactory = _mockingKernel.GetMock<INotificationViewModelFactory>();
            _mockingKernel.Bind<INotificationViewModelFactory>().ToConstant(_mockNotificationViewModelFactory.Object);
            _mockEventAggregator = new Mock<IEventAggregator>();
            _mockingKernel.Bind<IEventAggregator>().ToConstant(_mockEventAggregator.Object);

            _subject = _mockingKernel.Get<INotificationListViewModel>();
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void Constructor_WillInitializeNotificationsCollection()
        {
            _subject.Notifications.Should().NotBeNull();
        }

        [Test]
        public void Constructor_WillSubscribeInstanceToEventAggregator()
        {
            _mockEventAggregator.Verify(m => m.Subscribe(_subject));
        }

        [Test]
        public void WhenNewCloudClippingIsSaved_CreatesNewNotificationViewModel()
        {
            var firstClippingViewModel = new Mock<IClippingNotificationViewModel>();
            var secondClippingViewModel = new Mock<IClippingNotificationViewModel>();
            _mockNotificationViewModelFactory.SetupSequence(f => f.Create(It.IsAny<ClippingEntity>()))
                .Returns(Observable.Return(firstClippingViewModel.Object))
                .Returns(Observable.Return(secondClippingViewModel.Object));
            _mockClippingRepository.Setup(m => m.GetOperationObservable()).Returns(_testableClippingsObservable);
            _subject.Activate();

            _testScheduler.Start();

            _subject.Notifications.Should().OnlyContain(cvm => cvm == firstClippingViewModel.Object);
        }

        [Test]
        public void WhenACallIsSaved_CreatesNewNotificationViewModel()
        {
            var firstIncomingCallNotificationViewModel = new Mock<IIncomingCallNotificationViewModel>();
            var secondIncomingCallNotificationViewModel = new Mock<IIncomingCallNotificationViewModel>();
            _mockNotificationViewModelFactory.SetupSequence(f => f.Create(It.IsAny<RemotePhoneCallEntity>()))
                .Returns(Observable.Return(firstIncomingCallNotificationViewModel.Object))
                .Returns(Observable.Return(secondIncomingCallNotificationViewModel.Object));
            _mockCallRepository.Setup(m => m.GetOperationObservable()).Returns(_testableCallsObservable);
            _subject.Activate();

            _testScheduler.Start();

            _subject.Notifications.Should().OnlyContain(icvm => icvm == firstIncomingCallNotificationViewModel.Object);
        }

        [Test]
        public void WhenAMessageIsSaved_CreatesNewNotificationViewModel()
        {
            var firstIncomingSmsNotificationViewModel = new Mock<IIncomingSmsNotificationViewModel>();
            var secondIncomingSmsNotificationViewModel = new Mock<IIncomingSmsNotificationViewModel>();
            _mockNotificationViewModelFactory.SetupSequence(f => f.Create(It.IsAny<RemoteSmsMessageEntity>()))
                .Returns(Observable.Return(firstIncomingSmsNotificationViewModel.Object))
                .Returns(Observable.Return(secondIncomingSmsNotificationViewModel.Object));
            _mockMessageRepository.Setup(m => m.GetOperationObservable()).Returns(_testableMessagesObservable);
            _subject.Activate();

            _testScheduler.Start();

            _subject.Notifications.Should().OnlyContain(isn => isn == firstIncomingSmsNotificationViewModel.Object);
        }

        [Test]
        public void HandleWithDismissNotification_WhenNotificationExist_DismissesNotification()
        {
            const int Identifier = 42;
            var mockNotification = new Mock<INotificationViewModel> { DefaultValue = DefaultValue.Mock };
            mockNotification.SetupGet(m => m.Identifier).Returns(Identifier);
            _subject.Notifications.Add(mockNotification.Object);

            _subject.Handle(new DismissNotification(Identifier));

            mockNotification.Verify(m => m.Dismiss());
        }

        [Test]
        public void HandleWithDismissNotification_WhenNotificationDoesNotExist_DoesNotDismissOtherNotification()
        {
            var mockNotification = new Mock<INotificationViewModel> { DefaultValue = DefaultValue.Mock };
            mockNotification.SetupGet(m => m.Identifier).Returns(111);
            _subject.Notifications.Add(mockNotification.Object);

            _subject.Handle(new DismissNotification(42));

            mockNotification.Verify(m => m.Dismiss(), Times.Never());
        }
    }
}