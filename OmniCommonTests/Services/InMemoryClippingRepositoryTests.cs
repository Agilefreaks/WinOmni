using Moq;
using OmniCommon.Interfaces;

namespace OmniCommonTests.Services
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Domain;
    using OmniCommon.Services;

    public class InMemoryClippingRepositoryTests
    {
        private InMemoryClippingRepository _subject;
        private Mock<IDateTimeService> _mockDateTimeService;

        [SetUp]
        public void SetUp()
        {
            _mockDateTimeService = new Mock<IDateTimeService>();
            var delay = 1;
            _mockDateTimeService.SetupGet(m => m.UtcNow).Returns(() => DateTime.UtcNow.AddMinutes(delay++));
            _subject = new InMemoryClippingRepository(_mockDateTimeService.Object);
        }

        [Test]
        public void Save_Always_AddsClip()
        {
            var clipping = new Clipping("Test");

            _subject.Save(clipping);

            _subject.GetAll().Any(c => c.Content == clipping.Content).Should().BeTrue();
        }

        [Test]
        public void GetAll_Always_ReturnsClippingsCreatedInTheLast24Hours()
        {
            var clipping = new Clipping("asdf");
            _subject.Save(clipping);
            _mockDateTimeService.SetupGet(m => m.UtcNow).Returns(DateTime.UtcNow.AddHours(25));

            var clippings = _subject.GetAll();

            clippings.Should().BeEmpty();
        }

        [Test]
        public void GetAll_Always_ReturnsClippingsInReverseOrderThatTheyWereAdded()
        {
            var olderClipping = new Clipping("test1");
            var newerClipping = new Clipping("test2");
            _subject.Save(olderClipping);
            _subject.Save(newerClipping);

            var clippings = _subject.GetAll();

            clippings[0].Should().Be(newerClipping);
            clippings[1].Should().Be(olderClipping);
        }
    }
}
