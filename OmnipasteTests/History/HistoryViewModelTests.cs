using System.Collections.Generic;
using Caliburn.Micro;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OmniCommon.Domain;
using OmniCommon.Interfaces;
using OmniCommon.Services;
using Omnipaste.History;

namespace OmnipasteTests.History
{
    internal class HistoryViewModelWrapper : HistoryViewModel
    {
        public HistoryViewModelWrapper(IClippingRepository clippingRepository, IEventAggregator eventAggregator)
            : base(clippingRepository, eventAggregator)
        {
        }

        public void InvokeOnActivate()
        {
            base.OnActivate();
        }
    }

    [TestFixture]
    public class HistoryViewModelTests
    {
        private HistoryViewModelWrapper _subject;
        private Mock<IEventAggregator> _mockEventAggregator;
        private Mock<IClippingRepository> _mockClippingRepository;

        [SetUp]
        public void SetUp()
        {
            _mockEventAggregator = new Mock<IEventAggregator>();
            _mockClippingRepository = new Mock<IClippingRepository>();
            _subject = new HistoryViewModelWrapper(_mockClippingRepository.Object, _mockEventAggregator.Object);
        }

        [Test]
        public void Ctor_Always_CallsEventAggregatorSubscribe()
        {
            _mockEventAggregator.Verify(m => m.Subscribe(_subject));
        }

        [Test]
        public void HandleWithClipboardData_WhenClippingsHasLessThan5Elements_AddsClippingsToList()
        {
            var mockClipboardData = new Mock<IClipboardData>();
            mockClipboardData.Setup(m => m.GetData()).Returns("test");

            _subject.Handle(mockClipboardData.Object);

            _subject.RecentClippings.Should().Contain("test");
        }

        [Test]
        public void HandleWithClipboardData_WhenClippingsHas5Elements_RemovesFirstAddedElement()
        {
            var firstClipboardData = new ClipboardData(null, "1");
            _subject.Handle(firstClipboardData);
            _subject.Handle(new ClipboardData(null, "2"));
            _subject.Handle(new ClipboardData(null, "3"));
            _subject.Handle(new ClipboardData(null, "4"));
            _subject.Handle(new ClipboardData(null, "5"));

            _subject.Handle(new ClipboardData(null, "6"));

            _subject.RecentClippings.Should().NotContain(firstClipboardData);
        }

        [Test]
        public void OnActivate_Always_SetsClippingsFromRepository()
        {
            var clipping1 = new Clipping();
            var clipping2 = new Clipping();
            _mockClippingRepository.Setup(m => m.GetAll()).Returns(new List<Clipping> { clipping1, clipping2 });

            _subject.InvokeOnActivate();

            _subject.Clippings.Should().Contain(clipping1);
            _subject.Clippings.Should().Contain(clipping2);
        }
    }
}
