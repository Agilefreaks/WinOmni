﻿namespace OmnipasteTests.NotificationList
{
    using System;
    using System.Linq;
    using System.Reactive;
    using Caliburn.Micro;
    using Clipboard.Models;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Models;
    using Omnipaste.Notification;
    using Omnipaste.Notification.ClippingNotification;
    using Omnipaste.Notification.IncomingCallNotification;
    using Omnipaste.NotificationList;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class NotificationListViewModelTests
    {
        #region Fields

        private Mock<IClippingRepository> _mockClippingRepository;

        private Mock<ICallRepository> _mockCallRepository;

        private Mock<IMessageRepository> _mockMessageRepository;

        private MoqMockingKernel _mockingKernel;

        private INotificationListViewModel _subject;

        private TestScheduler _testScheduler;

        private ITestableObservable<RepositoryOperation<ClippingModel>> _testableClippingsObservable;

        private ITestableObservable<RepositoryOperation<Call>> _testableCallsObservable;

        private ITestableObservable<RepositoryOperation<SmsMessage>> _testableMessagesObservable;

        private Mock<INotificationViewModelFactory> _mockNotificationViewModelFactory;

        private Mock<IApplicationService> _mockApplicationService;

        private Mock<IEventAggregator> _mockEventAggregator;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MoqMockingKernel();

            _mockingKernel.Bind<IDisposable>().ToMock();

            SetupTestScheduler();

            _mockClippingRepository = new Mock<IClippingRepository> { DefaultValue = DefaultValue.Mock };
            _mockingKernel.Bind<IClippingRepository>().ToConstant(_mockClippingRepository.Object);
            _mockCallRepository = new Mock<ICallRepository> { DefaultValue = DefaultValue.Mock };
            _mockingKernel.Bind<ICallRepository>().ToConstant(_mockCallRepository.Object);
            _mockMessageRepository = new Mock<IMessageRepository> { DefaultValue = DefaultValue.Mock };
            _mockingKernel.Bind<IMessageRepository>().ToConstant(_mockMessageRepository.Object);
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
        public void WhenNewCloudClippingIsSaved_AddsNewNotificationViewModel()
        {
            _mockNotificationViewModelFactory.Setup(f => f.Create(It.IsAny<ClippingModel>()))
                .Returns(new Mock<IClippingNotificationViewModel>().Object);
            _mockClippingRepository.Setup(m => m.OperationObservable).Returns(_testableClippingsObservable);
            _subject.Activate();

            _testScheduler.Start();

            _subject.Notifications.Count.Should().Be(1);
        }

        [Test]
        public void WhenNewCloudClippingIsSaved_CreatesNewNotificationViewModel()
        {
            var mockClippingNotificationViewModel = new Mock<IClippingNotificationViewModel>();
            _mockNotificationViewModelFactory.Setup(f => f.Create(It.IsAny<ClippingModel>()))
                .Returns(mockClippingNotificationViewModel.Object);
            _mockClippingRepository.Setup(m => m.OperationObservable).Returns(_testableClippingsObservable);
            _subject.Activate();

            _testScheduler.Start();

            _subject.Notifications.First().Should().Be(mockClippingNotificationViewModel.Object);
        }

        [Test]
        public void WhenACallIsSaved_CreatesNewNotificationViewModel()
        {
            var mockIncomingCallNotificationViewModel = new Mock<IIncomingCallNotificationViewModel>();
            _mockNotificationViewModelFactory.Setup(f => f.Create(It.IsAny<Call>()))
                .Returns(mockIncomingCallNotificationViewModel.Object);
            _mockCallRepository.SetupGet(m => m.OperationObservable).Returns(_testableCallsObservable);
            _subject.Activate();

            _testScheduler.Start();

            _subject.Notifications.First().Should().Be(mockIncomingCallNotificationViewModel.Object);
        }

        [Test]
        public void WhenAMessageIsSaved_CreatesNewNotificationViewModel()
        {
            var mockIncomingCallNotificationViewModel = new Mock<IIncomingCallNotificationViewModel>();
            _mockNotificationViewModelFactory.Setup(f => f.Create(It.IsAny<SmsMessage>()))
                .Returns(mockIncomingCallNotificationViewModel.Object);
            _mockMessageRepository.SetupGet(m => m.OperationObservable).Returns(_testableMessagesObservable);
            _subject.Activate();

            _testScheduler.Start();

            _subject.Notifications.First().Should().Be(mockIncomingCallNotificationViewModel.Object);
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

        #endregion

        #region Methods

        private void SetupTestScheduler()
        {
            _testScheduler = new TestScheduler();
            _testableClippingsObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(
                        200,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingModel>(
                                RepositoryMethodEnum.Create,
                                new ClippingModel { Source = Clipping.ClippingSourceEnum.Cloud }))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(
                        250,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingModel>(
                                RepositoryMethodEnum.Create,
                                new ClippingModel { Source = Clipping.ClippingSourceEnum.Local }))));

            _testableCallsObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<Call>>>(
                        200,
                        Notification.CreateOnNext(
                            new RepositoryOperation<Call>(
                                RepositoryMethodEnum.Create,
                                new Call
                                    {
                                        Source = SourceType.Remote,
                                        ContactInfo = new ContactInfo { Phone = "phone number" }
                                    }))));
            _testableMessagesObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<SmsMessage>>>(
                        200,
                        Notification.CreateOnNext(
                            new RepositoryOperation<SmsMessage>(
                                RepositoryMethodEnum.Create,
                                new RemoteSmsMessage
                                    {
                                        ContactInfo = new ContactInfo { Phone = "phone number" }
                                    }))));

            SchedulerProvider.Dispatcher = _testScheduler;
        }

        #endregion
    }
}