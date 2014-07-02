﻿namespace OmnipasteTests.ClippingList
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
            public ClippingListViewModel(IObservable<Clipping> clippingsObservable)
                : base(clippingsObservable)
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
        public void NewClippingArrives_CreatesClippingViewModelAndInsertsItIntoClippingsCollection()
        {
            _fakeClippingSubject.OnNext(new Clipping());

            _subject.Clippings.Count.Should().Be(1);
        }
        [Test]
        public void NewClippingArrives_ThereAreOtherClippingsFromBefore_InsertsTheNewClippingAtTheStart()
        {
            var clipping = new Clipping();
            _subject.Clippings.Add(_mockingKernel.Get<IClippingViewModel>());
            var expectedClippingViewModel = new ClippingViewModel(clipping);
            _mockingKernel.Bind<IClippingViewModel>().ToConstant(expectedClippingViewModel);
            
            _fakeClippingSubject.OnNext(clipping);

            _subject.Clippings.First().Should().Be(expectedClippingViewModel);
        }
    }
}