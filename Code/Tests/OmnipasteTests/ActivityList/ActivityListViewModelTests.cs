namespace OmnipasteTests.ActivityList
{
    using System;
    using System.Linq;
    using System.Reactive;
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
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class ActivityListViewModelTests
    {
        private Mock<IActivityViewModelFactory> _mockActivityViewModelFactory;

        private Mock<IPhoneCallRepository> _mockCallRepository;

        private Mock<IClippingRepository> _mockClippingRepository;

        private Mock<IMessageRepository> _mockMessageRepository;

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
            _mockMessageRepository = new Mock<IMessageRepository> { DefaultValue = DefaultValue.Mock };
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
                _mockActivityViewModelFactory.Object);
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;
        }

        [Test]
        public void ReceivingAClipping_AfterActivate_CreatesANewActivityViewModelAndAddsItToItems()
        {
            var clippingOperationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(
                        100,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Changed, new ClippingModel()))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(
                        200,
                        Notification.CreateOnCompleted<RepositoryOperation<ClippingModel>>()));
            _mockClippingRepository.SetupGet(x => x.OperationObservable).Returns(clippingOperationObservable);
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
            _mockClippingRepository.SetupGet(x => x.OperationObservable).Returns(clippingOperationObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.Start();
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
            _mockClippingRepository.SetupGet(x => x.OperationObservable).Returns(clippingOperationObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.Start();
            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(1);
            _subject.Items.First().Model.BackingModel.Should().Be(modifiedClipping);
        }

        [Test]
        public void ReceivingACall_AfterActivate_CreatesANewActivityViewModelAndAddsItToItems()
        {
            var remoteRepositoryOperation = new RepositoryOperation<PhoneCall>(RepositoryMethodEnum.Changed, new RemotePhoneCall());
            var localRepositoryOperation = new RepositoryOperation<PhoneCall>(RepositoryMethodEnum.Changed, new LocalPhoneCall());
            var eventObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<PhoneCall>>>(100, Notification.CreateOnNext(remoteRepositoryOperation)),
                    new Recorded<Notification<RepositoryOperation<PhoneCall>>>(150, Notification.CreateOnNext(localRepositoryOperation)),
                    new Recorded<Notification<RepositoryOperation<PhoneCall>>>(200, Notification.CreateOnCompleted<RepositoryOperation<PhoneCall>>()));
            _mockCallRepository.SetupGet(m => m.OperationObservable).Returns(eventObservable);
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
                    new Recorded<Notification<RepositoryOperation<PhoneCall>>>(100, Notification.CreateOnNext(new RepositoryOperation<PhoneCall>(RepositoryMethodEnum.Changed, call))),
                    new Recorded<Notification<RepositoryOperation<PhoneCall>>>(200, Notification.CreateOnNext(new RepositoryOperation<PhoneCall>(RepositoryMethodEnum.Changed, modifiedCall))),
                    new Recorded<Notification<RepositoryOperation<PhoneCall>>>(300, Notification.CreateOnCompleted<RepositoryOperation<PhoneCall>>()));
            _mockCallRepository.SetupGet(m => m.OperationObservable).Returns(callObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceTo(1000);

            _subject.Items.Count.Should().Be(1);
            _subject.Items.First().Model.BackingModel.Should().Be(modifiedCall);
        }
    }
}