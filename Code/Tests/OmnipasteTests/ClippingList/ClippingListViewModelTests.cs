namespace OmnipasteTests.ClippingList
{
    using System;
    using System.Linq;
    using System.Reactive.Subjects;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Clipping;
    using Omnipaste.MasterClippingList.ClippingList;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using OmniUI.List;

    [TestFixture]
    public class ClippingListViewModelTests
    {
        private Subject<RepositoryOperation<ClippingModel>> _fakeClippingOperationSubject;
        
        private Mock<IClippingRepository> _mockClippingRepository;

        private ClippingListViewModel _subject;

        private MoqMockingKernel _mockingKernel;

        private TestScheduler _testScheduler;

        internal class ClippingListViewModel : ClippingListViewModelBase
        {
            public ClippingListViewModel(IClippingRepository clippingRepository)
                : base(clippingRepository)
            {
            }

            public override bool CanHandle(ClippingModel clipping)
            {
                return true;
            }
        }

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;

            _mockingKernel = new MoqMockingKernel();

            _fakeClippingOperationSubject = new Subject<RepositoryOperation<ClippingModel>>();
            _mockClippingRepository = new Mock<IClippingRepository> { DefaultValue = DefaultValue.Mock };
            _mockClippingRepository.SetupGet(m => m.OperationObservable).Returns(_fakeClippingOperationSubject);
            _mockingKernel.Bind<IClippingRepository>().ToConstant(_mockClippingRepository.Object);

            _subject = _mockingKernel.Get<ClippingListViewModel>();
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;
        }

        [Test]
        public void NewClippingArrives_CreatesClippingViewModelForItAndActivatesIt()
        {
            var clipping = new ClippingModel();
            ((IActivate)_subject).Activate();
            _testScheduler.Start();
            
            _fakeClippingOperationSubject.OnNext(new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Save, clipping));
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            _subject.Items.Count.Should().Be(1);
            _subject.Items[0].Model.Should().Be(clipping);
        }

        [Test]
        public void NewClippingArrives_ThereAreOtherClippingsFromBefore_InsertsTheNewClippingViewModelAtTheStart()
        {
            var clipping = new ClippingModel();
            var existingViewModel = new ClippingViewModel();
            _subject.Items.Add(existingViewModel);
            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            _fakeClippingOperationSubject.OnNext(new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Save, clipping));
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);
            
            _subject.Items.Count.Should().Be(2);
            _subject.Items.First().Model.Should().Be(clipping);
            _subject.Items.Last().Should().Be(existingViewModel);
        }

        [Test]
        public void Clippings_WhenIsEmpty_StatusIsEmpty()
        {
            _subject.Status.Should().Be(ListViewModelStatusEnum.Empty);
        }

        [Test]
        public void Clippings_WhenNotEmpty_StatusIsNotEmpty()
        {
            var clipping = new ClippingModel();
            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            _fakeClippingOperationSubject.OnNext(new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Save, clipping));
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            _subject.Status.Should().Be(ListViewModelStatusEnum.NotEmpty);
        }

        [Test]
        public void Clippings_BecomesEmpty_StatusIsEmpty()
        {
            var clipping = new ClippingModel();
            ((IActivate)_subject).Activate();
            _testScheduler.Start();
            _fakeClippingOperationSubject.OnNext(new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Save, clipping));
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            _subject.Items.Clear();

            _subject.Status.Should().Be(ListViewModelStatusEnum.Empty);
        }

        [Test]
        public void ActivateItem_Always_AddsItemsToTheBeginningOfTheList()
        {
            var clippingViewModel1 = new ClippingViewModel();
            var clippingViewModel2 = new ClippingViewModel();

            _subject.ActivateItem(clippingViewModel1);
            _subject.ActivateItem(clippingViewModel2);

            _subject.Items.IndexOf(clippingViewModel2).Should().Be(0);
            _subject.Items.IndexOf(clippingViewModel1).Should().Be(1);
        }

        [Test]
        public void ActivateItem_ItemsContains42ItemsAndTheLastItemCanClose_DeactivatesTheLastItemBeforeActivatingTheNewOne()
        {
            //This will be the last item since new items are put at the top
            var mockClippingViewModel = new Mock<IClippingViewModel>();
            mockClippingViewModel.Setup(x => x.CanClose(It.IsAny<Action<bool>>()))
                .Callback<Action<bool>>(action => action(true));
            ((IActivate)_subject).Activate();
            _subject.ActivateItem(mockClippingViewModel.Object);
            Enumerable.Range(0, ClippingListViewModel.MaxItemCount - 1)
                .Select(_ => new ClippingViewModel())
                .ToList()
                .ForEach(child => _subject.ActivateItem(child));

            _subject.ActivateItem(new ClippingViewModel());

            mockClippingViewModel.Verify(x => x.Deactivate(true), Times.Once());
            _subject.Items.Contains(mockClippingViewModel.Object).Should().BeFalse();
        }
    }
}
