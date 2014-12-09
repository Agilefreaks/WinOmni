namespace OmnipasteTests.ClippingList
{
    using System;
    using System.Linq;
    using System.Reactive.Subjects;
    using Clipboard.Models;
    using FluentAssertions;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste;
    using Omnipaste.Clipping;
    using Omnipaste.MasterClippingList.ClippingList;

    [TestFixture]
    public class ClippingListViewModelTests
    {
        private Subject<Clipping> _fakeClippingSubject;

        private ClippingListViewModel _subject;

        private MoqMockingKernel _mockingKernel;

        internal class ClippingListViewModel : ClippingListViewModelBase
        {
            public ClippingListViewModel(IObservable<Clipping> entityObservable)
                : base(entityObservable)
            {
            }
        }

        [SetUp]
        public void SetUp()
        {
            _mockingKernel = new MoqMockingKernel();

            _fakeClippingSubject = new Subject<Clipping>();
            _mockingKernel.Bind<IObservable<Clipping>>().ToConstant(_fakeClippingSubject);

            _subject = _mockingKernel.Get<ClippingListViewModel>();
        }

        [Test]
        public void NewClippingArrives_CreatesClippingViewModelForItAndActivatesIt()
        {
            var clipping = new Clipping();
            _fakeClippingSubject.OnNext(clipping);

            _subject.Items.Count.Should().Be(1);
            _subject.Items[0].Model.Should().Be(clipping);
        }

        [Test]
        public void NewClippingArrives_ThereAreOtherClippingsFromBefore_InsertsTheNewClippingViewModelAtTheStart()
        {
            var clipping = new Clipping();
            var existingViewModel = new ClippingViewModel();
            _subject.Items.Add(existingViewModel);

            _fakeClippingSubject.OnNext(clipping);

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
            var clipping = new Clipping();
            _fakeClippingSubject.OnNext(clipping);

            _subject.Status.Should().Be(ListViewModelStatusEnum.NotEmpty);
        }

        [Test]
        public void Clippings_BecomesEmpty_StatusIsEmpty()
        {
            var clipping = new Clipping();
            _fakeClippingSubject.OnNext(clipping);

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
