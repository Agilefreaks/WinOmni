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
    using Omnipaste.Activity;
    using Omnipaste.ActivityList;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Presenters.Factories;
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class ActivityListViewModelTests
    {
        private Mock<IActivityViewModelFactory> _mockActivityViewModelFactory;

        private Mock<IPhoneCallRepository> _mockCallRepository;

        private Mock<IClippingRepository> _mockClippingRepository;

        private Mock<ISmsMessageRepository> _mockMessageRepository;

        private Mock<IActivityPresenterFactory> _mockActivityPresenterFactory;

        private Mock<ISessionManager> _mockSessionManager;

        private Mock<IUiRefreshService> _mockUiRefreshService;

        private Mock<IUpdateInfoRepository> _mockUpdateInfoRepository;

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
            _mockActivityPresenterFactory = new Mock<IActivityPresenterFactory> { DefaultValue = DefaultValue.Mock };
            _mockUpdateInfoRepository = new Mock<IUpdateInfoRepository> { DefaultValue = DefaultValue.Mock };
            _mockActivityViewModelFactory = new Mock<IActivityViewModelFactory> { DefaultValue = DefaultValue.Mock };
            _mockUiRefreshService = new Mock<IUiRefreshService> { DefaultValue = DefaultValue.Mock };
            _mockClippingRepository = new Mock<IClippingRepository> { DefaultValue = DefaultValue.Mock };
            _mockSessionManager = new Mock<ISessionManager> { DefaultValue = DefaultValue.Mock };

            _mockActivityViewModelFactory.Setup(x => x.Create(It.IsAny<ActivityPresenter>()))
                .Returns<ActivityPresenter>(
                    presenter =>
                    new ActivityViewModel(_mockUiRefreshService.Object, _mockSessionManager.Object)
                        {
                            Model = presenter
                        });

            _subject = new ActivityListViewModel(
                _mockClippingRepository.Object, 
                _mockMessageRepository.Object, 
                _mockCallRepository.Object, 
                _mockUpdateInfoRepository.Object, 
                _mockActivityPresenterFactory.Object, 
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
            var remotePhoneCall = new RemotePhoneCall();
            var phoneCallObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<IEnumerable<RemotePhoneCall>>>(100, Notification.CreateOnNext((IEnumerable<RemotePhoneCall>)new List<RemotePhoneCall> { remotePhoneCall })));
            var remoteSmsMessage = new RemoteSmsMessage();
            var smsMessageObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<IEnumerable<RemoteSmsMessage>>>(200, Notification.CreateOnNext((IEnumerable<RemoteSmsMessage>)new List<RemoteSmsMessage> { remoteSmsMessage })));
            _mockMessageRepository.Setup(m => m.GetAll()).Returns(smsMessageObservable);
            _mockCallRepository.Setup(m => m.GetAll()).Returns(phoneCallObservable);
            SetupPhoneCallActivityPresenterFactory<RemotePhoneCallPresenter>(remotePhoneCall);
            SetupSmsMessageActivityPresenterFactory<RemoteSmsMessagePresenter>(remoteSmsMessage);

            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(300);

            _subject.Items.Count.Should().Be(2);
        }

        [Test]
        public void ReceivingAClipping_AfterActivate_CreatesANewActivityViewModelAndAddsItToItems()
        {
            var clippingModel = new ClippingModel();
            var clippingOperationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(
                        100,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Changed, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(
                        200,
                        Notification.CreateOnCompleted<RepositoryOperation<ClippingModel>>()));
            _mockClippingRepository.Setup(x => x.GetOperationObservable()).Returns(clippingOperationObservable);
            SetupClippingActivityPresenterFactory<ClippingPresenter>(clippingModel);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.Items.Count.Should().Be(1);
        }

        [Test]
        public void RemovingAClipping_AfterActivateWhenClippingWasPreviouslyReceived_RemovesViewModelForClipping()
        {
            const string SourceId = "42";
            var clippingModel = new ClippingModel { UniqueId = SourceId };
            var clippingOperationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(
                        100,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Changed, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(
                        200,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Delete, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(
                        300,
                        Notification.CreateOnCompleted<RepositoryOperation<ClippingModel>>()));
            _mockClippingRepository.Setup(x => x.GetOperationObservable()).Returns(clippingOperationObservable);
            SetupClippingActivityPresenterFactory<ClippingPresenter>(clippingModel);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(0);
        }

        [Test]
        public void UpdatingAClipping_AfterActivateWhenClippingWasPreviouslyReceived_UpdatesViewModelWithNewClipping()
        {
            const string SourceId = "42";
            var clippingModel = new ClippingModel { UniqueId = SourceId };
            var modifiedClipping = new ClippingModel { UniqueId = SourceId, Content = "Test" };
            var clippingOperationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(
                        100,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Changed, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(
                        200,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Changed, modifiedClipping))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(
                        300,
                        Notification.CreateOnCompleted<RepositoryOperation<ClippingModel>>()));
            _mockClippingRepository.Setup(x => x.GetOperationObservable()).Returns(clippingOperationObservable);
            SetupClippingActivityPresenterFactory<ClippingPresenter>(clippingModel);
            SetupClippingActivityPresenterFactory<ClippingPresenter>(modifiedClipping);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(1);
            _subject.Items.First().Model.BackingModel.BackingModel.Should().Be(modifiedClipping);
        }

        [Test]
        public void ReceivingACall_AfterActivate_CreatesANewActivityViewModelAndAddsItToItems()
        {
            var remotePhoneCall = new RemotePhoneCall();
            var remoteRepositoryOperation = new RepositoryOperation<RemotePhoneCall>(RepositoryMethodEnum.Changed, remotePhoneCall);
            var eventObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<RemotePhoneCall>>>(100, Notification.CreateOnNext(remoteRepositoryOperation)),
                    new Recorded<Notification<RepositoryOperation<RemotePhoneCall>>>(200, Notification.CreateOnCompleted<RepositoryOperation<RemotePhoneCall>>()));
            _mockCallRepository.Setup(m => m.GetOperationObservable<RemotePhoneCall>()).Returns(eventObservable);
            SetupPhoneCallActivityPresenterFactory<RemotePhoneCallPresenter>(remotePhoneCall);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.Items.Count.Should().Be(1);
        }

        [Test]
        public void UpdatingACall_AfterActivateWhenPreviouslyReceived_UpdatesViewModelWithNewCall()
        {
            const string UniqueId = "42";
            var call = new RemotePhoneCall { UniqueId = UniqueId };
            var modifiedCall = new RemotePhoneCall { UniqueId = UniqueId, Content = "Test" };
            var callObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<RemotePhoneCall>>>(100, Notification.CreateOnNext(new RepositoryOperation<RemotePhoneCall>(RepositoryMethodEnum.Changed, call))),
                    new Recorded<Notification<RepositoryOperation<RemotePhoneCall>>>(200, Notification.CreateOnNext(new RepositoryOperation<RemotePhoneCall>(RepositoryMethodEnum.Changed, modifiedCall))),
                    new Recorded<Notification<RepositoryOperation<RemotePhoneCall>>>(300, Notification.CreateOnCompleted<RepositoryOperation<RemotePhoneCall>>()));
            _mockCallRepository.Setup(m => m.GetOperationObservable<RemotePhoneCall>()).Returns(callObservable);
            SetupPhoneCallActivityPresenterFactory<RemotePhoneCallPresenter>(call);
            SetupPhoneCallActivityPresenterFactory<RemotePhoneCallPresenter>(modifiedCall);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceTo(1000);

            _subject.Items.Count.Should().Be(1);
            _subject.Items.First().Model.BackingModel.BackingModel.Should().Be(modifiedCall);
        }

        private void SetupClippingActivityPresenterFactory<TPresenter>(ClippingModel model)
            where TPresenter : Presenter
        {
            var presenter = (TPresenter)Activator.CreateInstance(typeof(TPresenter), model);
            var mock = new Mock<ActivityPresenter>();

            mock.SetupGet(m => m.BackingModel).Returns(presenter);
            mock.SetupGet(m => m.Type).Returns(ActivityTypeEnum.Clipping);
            mock.SetupGet(m => m.SourceId).Returns(presenter.UniqueId);
            _mockActivityPresenterFactory.Setup(m => m.Create(model)).Returns(Observable.Return(mock.Object));
        }

        private void SetupPhoneCallActivityPresenterFactory<TPresenter>(PhoneCall model)
            where TPresenter : Presenter
        {
            var presenter = (TPresenter)Activator.CreateInstance(typeof(TPresenter), model);
            var mock = new Mock<ActivityPresenter>();

            mock.SetupGet(m => m.BackingModel).Returns(presenter);
            mock.SetupGet(m => m.SourceId).Returns(presenter.UniqueId);
            _mockActivityPresenterFactory.Setup(m => m.Create(model)).Returns(Observable.Return(mock.Object));
        }

        private void SetupSmsMessageActivityPresenterFactory<TPresenter>(SmsMessage model)
            where TPresenter : Presenter
        {
            var presenter = (TPresenter)Activator.CreateInstance(typeof(TPresenter), model);
            var mock = new Mock<ActivityPresenter>();

            mock.SetupGet(m => m.BackingModel).Returns(presenter);
            mock.SetupGet(m => m.SourceId).Returns(presenter.UniqueId);
            _mockActivityPresenterFactory.Setup(m => m.Create(model)).Returns(Observable.Return(mock.Object));
        }

    }
}