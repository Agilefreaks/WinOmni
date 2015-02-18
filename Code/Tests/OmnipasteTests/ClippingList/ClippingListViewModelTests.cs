namespace OmnipasteTests.ClippingList
{
    using System.Linq;
    using System.Reactive;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.ClippingList;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class ClippingListViewModelTests
    {
        private ClippingListViewModel _subject;

        private Mock<IClippingViewModelFactory> _mockClippingViewModelFactory;

        private Mock<ISessionManager> _mockSessionManager;

        private Mock<IClippingRepository> _mockClippingRepository;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;

            _mockClippingViewModelFactory = new Mock<IClippingViewModelFactory>();
            _mockSessionManager = new Mock<ISessionManager> { DefaultValue = DefaultValue.Mock };
            _mockClippingRepository = new Mock<IClippingRepository> { DefaultValue = DefaultValue.Mock };
            _mockClippingViewModelFactory.Setup(m => m.Create(It.IsAny<ClippingPresenter>()))
                .Returns<ClippingPresenter>(clippingModel => new ClippingViewModel(_mockSessionManager.Object) { Model = clippingModel });
            _subject = new ClippingListViewModel(_mockClippingRepository.Object, _mockClippingViewModelFactory.Object);
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;
        }

        [Test]
        public void ReceivingAClipping_AfterActivate_CreatesANewViewModelAndAddsItToItems()
        {
            var clippingOperationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(100,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Create, new ClippingModel()))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(200,
                        Notification.CreateOnCompleted<RepositoryOperation<ClippingModel>>()));
            _mockClippingRepository.SetupGet(x => x.OperationObservable).Returns(clippingOperationObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(1);
        }

        [Test]
        public void RemovingAClipping_AfterActivateWhenClippingWasPreviouslyReceived_RemovesViewModelForClipping()
        {
            const string SourceId = "42";
            var clippingModel = new ClippingModel { UniqueId = SourceId };
            var clippingOperationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(100, Notification.CreateOnNext(new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Create, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(200, Notification.CreateOnNext(new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Delete, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(300, Notification.CreateOnCompleted<RepositoryOperation<ClippingModel>>()));
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
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(100, Notification.CreateOnNext(new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Create, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(200, Notification.CreateOnNext(new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Update, modifiedClipping))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(300, Notification.CreateOnCompleted<RepositoryOperation<ClippingModel>>()));
            _mockClippingRepository.SetupGet(x => x.OperationObservable).Returns(clippingOperationObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.Start();
            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(1);
            _subject.Items.First().Model.BackingModel.Should().Be(modifiedClipping);
        }
    }
}
