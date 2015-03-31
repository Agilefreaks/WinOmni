namespace OmnipasteTests.ClippingList
{
    using System.Linq;
    using System.Reactive;
    using Caliburn.Micro;
    using Clipboard.Dto;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.ClippingList;
    using Omnipaste.Entities;
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
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(100,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingEntity>(RepositoryMethodEnum.Changed, new ClippingEntity()))),
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(200,
                        Notification.CreateOnCompleted<RepositoryOperation<ClippingEntity>>()));
            _mockClippingRepository.Setup(x => x.GetOperationObservable()).Returns(clippingOperationObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(1);
        }

        [Test]
        public void RemovingAClipping_AfterActivateWhenClippingWasPreviouslyReceived_RemovesViewModelForClipping()
        {
            const string SourceId = "42";
            var clippingModel = new ClippingEntity { UniqueId = SourceId };
            var clippingOperationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(100, Notification.CreateOnNext(new RepositoryOperation<ClippingEntity>(RepositoryMethodEnum.Changed, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(200, Notification.CreateOnNext(new RepositoryOperation<ClippingEntity>(RepositoryMethodEnum.Delete, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(300, Notification.CreateOnCompleted<RepositoryOperation<ClippingEntity>>()));
            _mockClippingRepository.Setup(x => x.GetOperationObservable()).Returns(clippingOperationObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.Start();
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
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(100, Notification.CreateOnNext(new RepositoryOperation<ClippingEntity>(RepositoryMethodEnum.Changed, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(200, Notification.CreateOnNext(new RepositoryOperation<ClippingEntity>(RepositoryMethodEnum.Changed, modifiedClipping))),
                    new Recorded<Notification<RepositoryOperation<ClippingEntity>>>(300, Notification.CreateOnCompleted<RepositoryOperation<ClippingEntity>>()));
            _mockClippingRepository.Setup(x => x.GetOperationObservable()).Returns(clippingOperationObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.Start();
            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(1);
            _subject.Items.First().Model.BackingModel.Should().Be(modifiedClipping);
        }

        [Test]
        public void FilteredItems_NoFilterIsApplied_ReturnsAllItems()
        {
            var clippingPresenter1 = new ClippingPresenter(new ClippingEntity { Content = "test1" });
            var clippingPresenter2 = new ClippingPresenter(new ClippingEntity { Content = "Other" });
            ((IActivate)_subject).Activate();
            _subject.ActivateItem(new ClippingViewModel(_mockSessionManager.Object) { Model = clippingPresenter1});
            _subject.ActivateItem(new ClippingViewModel(_mockSessionManager.Object) { Model = clippingPresenter2});

            _subject.FilteredItems.Count.Should().Be(2);
        }

        [Test]
        public void FilteredItems_WhenFilterTextHasValue_FiltersItemsByContentContainingText()
        {
            var clippingPresenter1 = new ClippingPresenter(new ClippingEntity { Content = "test1" });
            var clippingPresenter2 = new ClippingPresenter(new ClippingEntity { Content = "Other" });
            ((IActivate)_subject).Activate();
            _subject.ActivateItem(new ClippingViewModel(_mockSessionManager.Object) { Model = clippingPresenter1});
            _subject.ActivateItem(new ClippingViewModel(_mockSessionManager.Object) { Model = clippingPresenter2});

            _subject.FilterText = "oth";

            _subject.FilteredItems.Count.Should().Be(1);
            _subject.FilteredItems.Cast<IClippingViewModel>().First().Model.Should().Be(clippingPresenter2);
        }

        [Test]
        public void FilteredItems_WhenAllFilterOptionsAreFalse_ReturnsAllItems()
        {
            AddClippingsWithFilterCombinations();

            _subject.ShowCloudClippings = false;
            _subject.ShowLocalClippings = false;
            _subject.ShowStarred = false;

            _subject.FilteredItems.Count.Should().Be(4);
        }

        [Test]
        public void FilteredItems_WhenShowCloudClippingsIsSelected_ReturnsOnlyCloudClippings()
        {
            AddClippingsWithFilterCombinations();

            _subject.ShowCloudClippings = true;
            _subject.ShowLocalClippings = false;
            _subject.ShowStarred = false;

            _subject.FilteredItems.Count.Should().Be(2);
            _subject.FilteredItems.Cast<IClippingViewModel>()
                .All(vm => vm.Model.BackingModel.Source == ClippingDto.ClippingSourceEnum.Cloud)
                .Should()
                .BeTrue();
        }

        [Test]
        public void FilteredItems_WhenShowLocalClippingsIsSelected_ReturnsOnlyLocalClippings()
        {
            AddClippingsWithFilterCombinations();

            _subject.ShowCloudClippings = false;
            _subject.ShowLocalClippings = true;
            _subject.ShowStarred = false;

            _subject.FilteredItems.Count.Should().Be(2);
            _subject.FilteredItems.Cast<IClippingViewModel>()
                .All(vm => vm.Model.BackingModel.Source == ClippingDto.ClippingSourceEnum.Local)
                .Should()
                .BeTrue();
        }

        [Test]
        public void FilteredItems_WhenShowCloudClippingsAndShowStarredAreSelected_ReturnsOnlyStarredCloudClippings()
        {
            AddClippingsWithFilterCombinations();

            _subject.ShowCloudClippings = true;
            _subject.ShowLocalClippings = false;
            _subject.ShowStarred = true;

            _subject.FilteredItems.Count.Should().Be(1);
            _subject.FilteredItems.Cast<IClippingViewModel>()
                .All(vm => vm.Model.BackingModel.Source == ClippingDto.ClippingSourceEnum.Cloud && vm.Model.IsStarred)
                .Should()
                .BeTrue();
        }

        private void AddClippingsWithFilterCombinations()
        {
            _subject.ActivateItem(new ClippingViewModel(_mockSessionManager.Object) { Model = new ClippingPresenter(new ClippingEntity { Content = "1", IsStarred = false, Source = ClippingDto.ClippingSourceEnum.Cloud })});
            _subject.ActivateItem(new ClippingViewModel(_mockSessionManager.Object) { Model = new ClippingPresenter(new ClippingEntity { Content = "2", IsStarred = true, Source = ClippingDto.ClippingSourceEnum.Local })});
            _subject.ActivateItem(new ClippingViewModel(_mockSessionManager.Object) { Model = new ClippingPresenter(new ClippingEntity { Content = "3", IsStarred = true, Source = ClippingDto.ClippingSourceEnum.Cloud })});
            _subject.ActivateItem(new ClippingViewModel(_mockSessionManager.Object) { Model = new ClippingPresenter(new ClippingEntity { Content = "4", IsStarred = false, Source = ClippingDto.ClippingSourceEnum.Local })});
        }
    }
}
