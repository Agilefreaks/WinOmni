namespace OmnipasteTests.ClippingList
{
    using System;
    using System.Linq;
    using System.Reactive.Subjects;
    using Clipboard.Models;
    using FluentAssertions;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste;
    using Omnipaste.Clipping;
    using Omnipaste.ClippingList;

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
        public void Clippings_ShouldBeLimittedToACountOf42()
        {
            ((LimitableBindableCollection<IClippingViewModel>)_subject.ViewModels).Limit.Should().Be(42);
        }

        [Test]
        public void NewClippingArrives_CreatesClippingViewModelAndInsertsItIntoClippingsCollection()
        {
            _fakeClippingSubject.OnNext(new Clipping());

            _subject.ViewModels.Count.Should().Be(1);
        }

        [Test]
        public void NewClippingArrives_ThereAreOtherClippingsFromBefore_InsertsTheNewClippingAtTheStart()
        {
            var clipping = new Clipping();
            _subject.ViewModels.Add(_mockingKernel.Get<IClippingViewModel>());
            var expectedClippingViewModel = new ClippingViewModel(clipping);
            _mockingKernel.Bind<IClippingViewModel>().ToConstant(expectedClippingViewModel);

            _fakeClippingSubject.OnNext(clipping);

            _subject.ViewModels.First().Should().Be(expectedClippingViewModel);
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

            _subject.ViewModels.Clear();

            _subject.Status.Should().Be(ListViewModelStatusEnum.Empty);
        }
    }
}
