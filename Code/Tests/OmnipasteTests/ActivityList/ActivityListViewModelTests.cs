namespace OmnipasteTests.ActivityList
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
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.ActivityList;
    using Omnipaste.ActivityList.Activity;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Models.Factories;
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class ActivityListViewModelTests
    {
        private Mock<IActivityViewModelFactory> _mockActivityViewModelFactory;

        private Mock<IPhoneCallRepository> _mockCallRepository;

        private Mock<IClippingRepository> _mockClippingRepository;

        private Mock<ISmsMessageRepository> _mockMessageRepository;

        private Mock<IActivityModelFactory> _mockActivityModelFactory;

        private Mock<ISessionManager> _mockSessionManager;

        private Mock<IUiRefreshService> _mockUiRefreshService;

        private Mock<IUpdateRepository> _mockUpdateRepository;

        private ActivityListViewModel _subject;

        private TestScheduler _testScheduler;

        [SetUp]
        public void Setup()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;

            _mockCallRepository = new Mock<IPhoneCallRepository> { DefaultValue = DefaultValue.Mock };
            _mockMessageRepository = new Mock<ISmsMessageRepository> { DefaultValue = DefaultValue.Mock };
            _mockActivityModelFactory = new Mock<IActivityModelFactory> { DefaultValue = DefaultValue.Mock };
            _mockUpdateRepository = new Mock<IUpdateRepository> { DefaultValue = DefaultValue.Mock };
            _mockActivityViewModelFactory = new Mock<IActivityViewModelFactory> { DefaultValue = DefaultValue.Mock };
            _mockUiRefreshService = new Mock<IUiRefreshService> { DefaultValue = DefaultValue.Mock };
            _mockClippingRepository = new Mock<IClippingRepository> { DefaultValue = DefaultValue.Mock };
            _mockSessionManager = new Mock<ISessionManager> { DefaultValue = DefaultValue.Mock };

            _mockActivityViewModelFactory.Setup(x => x.Create(It.IsAny<ActivityModel>()))
                .Returns<ActivityModel>(
                    model =>
                    new ActivityViewModel(_mockUiRefreshService.Object, _mockSessionManager.Object)
                        {
                            Model = model
                        });

            _subject = new ActivityListViewModel(
                _mockClippingRepository.Object, 
                _mockMessageRepository.Object, 
                _mockCallRepository.Object, 
                _mockUpdateRepository.Object, 
                _mockActivityModelFactory.Object, 
                _mockActivityViewModelFactory.Object);
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;
        }

        [Test]
        public void Activate_Always_FetchesItems()
        {
            var remotePhoneCall = new RemotePhoneCallEntity();
            var phoneCallObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<IEnumerable<RemotePhoneCallEntity>>>(100, Notification.CreateOnNext((IEnumerable<RemotePhoneCallEntity>)new List<RemotePhoneCallEntity> { remotePhoneCall })));
            var remoteSmsMessage = new RemoteSmsMessageEntity();
            var smsMessageObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<IEnumerable<RemoteSmsMessageEntity>>>(200, Notification.CreateOnNext((IEnumerable<RemoteSmsMessageEntity>)new List<RemoteSmsMessageEntity> { remoteSmsMessage })));
            _mockMessageRepository.Setup(m => m.GetAll()).Returns(smsMessageObservable);
            _mockCallRepository.Setup(m => m.GetAll()).Returns(phoneCallObservable);
            SetupPhoneCallActivityModelFactory(remotePhoneCall);
            SetupSmsMessageActivityModelFactory(remoteSmsMessage);

            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(300);

            _subject.Items.Count.Should().Be(2);
        }

        [Test]
        public void ReceivingAClipping_AfterActivate_CreatesANewActivityViewModelAndAddsItToItems()
        {
            var clippingModel = new ClippingEntity();
            var clippingOperationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(
                        100,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingEntity>(RepositoryMethodEnum.Changed, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(
                        200,
                        Notification.CreateOnCompleted<RepositoryOperation<ClippingEntity>>()));
            _mockClippingRepository.Setup(x => x.GetOperationObservable()).Returns(clippingOperationObservable);
            SetupClippingActivityModelFactory(clippingModel);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.Items.Count.Should().Be(1);
        }

        [Test]
        public void RemovingAClipping_AfterActivateWhenClippingWasPreviouslyReceived_RemovesViewModelForClipping()
        {
            var clippingModel = new ClippingEntity { UniqueId = "42" };
            var clippingOperationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(
                        100,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingEntity>(RepositoryMethodEnum.Changed, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(
                        200,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingEntity>(RepositoryMethodEnum.Delete, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(
                        300,
                        Notification.CreateOnCompleted<RepositoryOperation<ClippingEntity>>()));
            _mockClippingRepository.Setup(x => x.GetOperationObservable()).Returns(clippingOperationObservable);
            SetupClippingActivityModelFactory(clippingModel);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(0);
        }

        [Test]
        public void UpdatingAClipping_AfterActivateWhenClippingWasPreviouslyReceived_UpdatesViewModelWithNewClipping()
        {
            const string SourceId = "42";
            var clippingModel = new ClippingEntity { UniqueId = SourceId };
            var modifiedClipping = new ClippingEntity { UniqueId = SourceId, Content = "Test" };
            var clippingOperationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(
                        100,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingEntity>(RepositoryMethodEnum.Changed, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(
                        200,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingEntity>(RepositoryMethodEnum.Changed, modifiedClipping))),
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(
                        300,
                        Notification.CreateOnCompleted<RepositoryOperation<ClippingEntity>>()));
            _mockClippingRepository.Setup(x => x.GetOperationObservable()).Returns(clippingOperationObservable);
            SetupClippingActivityModelFactory(clippingModel);
            SetupClippingActivityModelFactory(modifiedClipping);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(1);
            _subject.Items.First().Model.BackingEntity.Should().Be(modifiedClipping);
        }

        [Test]
        public void ReceivingACall_AfterActivate_CreatesANewActivityViewModelAndAddsItToItems()
        {
            var remotePhoneCall = new RemotePhoneCallEntity();
            var remoteRepositoryOperation = new RepositoryOperation<RemotePhoneCallEntity>(RepositoryMethodEnum.Changed, remotePhoneCall);
            var eventObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<RemotePhoneCallEntity>>>(100, Notification.CreateOnNext(remoteRepositoryOperation)),
                    new Recorded<Notification<RepositoryOperation<RemotePhoneCallEntity>>>(200, Notification.CreateOnCompleted<RepositoryOperation<RemotePhoneCallEntity>>()));
            _mockCallRepository.Setup(m => m.GetOperationObservable<RemotePhoneCallEntity>()).Returns(eventObservable);
            SetupPhoneCallActivityModelFactory(remotePhoneCall);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.Items.Count.Should().Be(1);
        }

        [Test]
        public void UpdatingACall_AfterActivateWhenPreviouslyReceived_UpdatesViewModelWithNewCall()
        {
            const string UniqueId = "42";
            var call = new RemotePhoneCallEntity { UniqueId = UniqueId };
            var modifiedCall = new RemotePhoneCallEntity { UniqueId = UniqueId, Content = "Test" };
            var callObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<RemotePhoneCallEntity>>>(100, Notification.CreateOnNext(new RepositoryOperation<RemotePhoneCallEntity>(RepositoryMethodEnum.Changed, call))),
                    new Recorded<Notification<RepositoryOperation<RemotePhoneCallEntity>>>(200, Notification.CreateOnNext(new RepositoryOperation<RemotePhoneCallEntity>(RepositoryMethodEnum.Changed, modifiedCall))),
                    new Recorded<Notification<RepositoryOperation<RemotePhoneCallEntity>>>(300, Notification.CreateOnCompleted<RepositoryOperation<RemotePhoneCallEntity>>()));
            _mockCallRepository.Setup(m => m.GetOperationObservable<RemotePhoneCallEntity>()).Returns(callObservable);
            SetupPhoneCallActivityModelFactory(call);
            SetupPhoneCallActivityModelFactory(modifiedCall);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceTo(1000);

            _subject.Items.Count.Should().Be(1);
            _subject.Items.First().Model.BackingEntity.Should().Be(modifiedCall);
        }

        private void SetupClippingActivityModelFactory(ClippingEntity entity)
        {
            var activityModel = ActivityModel.BeginBuild(entity).WithType(ActivityTypeEnum.Clipping).Build();

            _mockActivityModelFactory.Setup(m => m.Create(entity)).Returns(Observable.Return(activityModel));
        }

        private void SetupPhoneCallActivityModelFactory(PhoneCallEntity model)
        {
            var activityModel = ActivityModel.BeginBuild(model).WithType(ActivityTypeEnum.Call).Build();

            _mockActivityModelFactory.Setup(m => m.Create(model)).Returns(Observable.Return(activityModel));
        }

        private void SetupSmsMessageActivityModelFactory(SmsMessageEntity model)
        {
            var activityModel = ActivityModel.BeginBuild(model).WithType(ActivityTypeEnum.Message).Build();

            _mockActivityModelFactory.Setup(m => m.Create(model)).Returns(Observable.Return(activityModel));
        }

    }
}