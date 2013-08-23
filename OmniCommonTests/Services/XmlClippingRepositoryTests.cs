namespace OmniCommonTests.Services
{
    using System;
    using Moq;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Domain;
    using OmniCommon.Interfaces;
    using OmniCommon.Services;

    [TestFixture]
    public class XmlClippingRepositoryTests
    {
        private XmlClippingRepository _subject;
        private SystemXmlSerializer _xmlSerializer;
        private FileService _fileService;
        private Mock<IDateTimeService> _mockDateTimeService;

        [SetUp]
        public void SetUp()
        {
            _xmlSerializer = new SystemXmlSerializer();
            _fileService = new FileService();
            _mockDateTimeService = new Mock<IDateTimeService>();
            var delay = 1;
            _mockDateTimeService.SetupGet(m => m.UtcNow).Returns(() => DateTime.UtcNow.AddMinutes(delay++));
            _subject = new XmlClippingRepository(_fileService, _xmlSerializer, _mockDateTimeService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_subject.FilePath))
            {
                File.Delete(_subject.FilePath);
            }
        }

        [Test]
        public void Save_Always_AddsClip()
        {
            var clipping = new Clipping("Test");
            
            _subject.Save(clipping);

            _subject.GetForLast24Hours().Any(c => c.Content == clipping.Content).Should().BeTrue();
        }

        [Test]
        public void Save_Always_SetsDateCreated()
        {
            var clipping = new Clipping("Test");
            var currentDate = DateTime.UtcNow;
            _mockDateTimeService.SetupGet(m => m.UtcNow).Returns(currentDate);

            _subject.Save(clipping);

            clipping.DateCreated.Should().Be(currentDate);
        }

        [Test]
        public void GetAll_Always_ReturnsClippingsCreatedInTheLast24Hours()
        {
            var clipping = new Clipping("asdf");
            _subject.Save(clipping);
            _mockDateTimeService.SetupGet(m => m.UtcNow).Returns(DateTime.UtcNow.AddHours(25));

            var clippings = _subject.GetForLast24Hours();

            clippings.Should().BeEmpty();
        }

        [Test]
        public void GetAll_Always_ReturnsClippingsInReverseOrderThatTheyWereAdded()
        {
            var olderClipping = new Clipping("test1");
            var newerClipping = new Clipping("test2");
            _subject.Save(olderClipping);
            _subject.Save(newerClipping);

            var clippings = _subject.GetForLast24Hours();

            clippings[0].Content.Should().Be(newerClipping.Content);
            clippings[1].Content.Should().Be(olderClipping.Content);
        }
    }
}
