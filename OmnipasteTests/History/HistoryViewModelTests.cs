using Caliburn.Micro;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OmniCommon.Interfaces;
using OmniCommon.Services;
using Omnipaste.History;

namespace OmnipasteTests.History
{
    [TestFixture]
    public class HistoryViewModelTests
    {
        private HistoryViewModel _subject;
        private Mock<IEventAggregator> _mockEventAggregator;

        [SetUp]
        public void SetUp()
        {
            _mockEventAggregator = new Mock<IEventAggregator>();
            _subject = new HistoryViewModel(_mockEventAggregator.Object);
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

            _subject.Clippings.Should().Contain("test");
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

            _subject.Clippings.Should().NotContain(firstClipboardData);
        }
    }
}
