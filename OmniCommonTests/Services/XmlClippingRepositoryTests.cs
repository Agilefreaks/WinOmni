using System.Collections.Generic;

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
        public void GetForLast24Hours_Always_ReturnsClippingsCreatedInTheLast24Hours()
        {
            var clipping = new Clipping("asdf");
            _subject.Save(clipping);
            _mockDateTimeService.SetupGet(m => m.UtcNow).Returns(DateTime.UtcNow.AddHours(25));

            var clippings = _subject.GetForLast24Hours();

            clippings.Should().BeEmpty();
        }

        [Test]
        public void GetForLast24Hours_Always_ReturnsClippingsInReverseOrderThatTheyWereAdded()
        {
            var olderClipping = new Clipping("test1");
            var newerClipping = new Clipping("test2");
            _subject.Save(olderClipping);
            _subject.Save(newerClipping);

            var clippings = _subject.GetForLast24Hours();

            clippings[0].Content.Should().Be(newerClipping.Content);
            clippings[1].Content.Should().Be(olderClipping.Content);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetForLast24Hours_WhenFileServiceRaisesException_Throws()
        {
            var mockFileService = new Mock<IFileService>();
            mockFileService.Setup(m => m.Exists(It.IsAny<string>()))
                           .Throws<ArgumentException>();
            mockFileService.SetupGet(m => m.AppDataDir).Returns(Environment.CurrentDirectory);
            _subject.FileService = mockFileService.Object;

            _subject.GetForLast24Hours();
        }

        [Test]
        public void GetForLast24Hours_WhenSerializerRaisesException_AlwaysCallsStreamDispose()
        {
            var mockFileService = new Mock<IFileService>();
            mockFileService.Setup(m => m.Exists(It.IsAny<string>()))
                           .Returns(true);
            mockFileService.SetupGet(m => m.AppDataDir).Returns(Environment.CurrentDirectory);
            var mockStream = new Mock<Stream>();
            mockFileService.Setup(m => m.Open(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>()))
                           .Returns(mockStream.Object);
            var mockSerializer = new Mock<IXmlSerializer>();
            mockSerializer.Setup(m => m.Serialize(mockStream.Object, It.IsAny<object>()))
                          .Throws<Exception>();
            _subject.FileService = mockFileService.Object;
            _subject.Serializer = mockSerializer.Object;

            try
            {
                _subject.GetForLast24Hours();
                Assert.Fail("this should throw");
            }
            catch
            {
                mockStream.Verify(m => m.Close());
            }
        }

        [Test]
        public void Save_WhenSerializeFails_CallsStreamClose()
        {
            var mockSerializer = new Mock<IXmlSerializer>();
            var clippings = new List<Clipping>();
            mockSerializer.Setup(m => m.Deserialize<List<Clipping>>(It.IsAny<Stream>()))
                          .Returns(clippings);
            mockSerializer.Setup(m => m.Serialize(It.IsAny<Stream>(), clippings))
                          .Throws<Exception>();
            _subject.Serializer = mockSerializer.Object;
            var mockFileService = new Mock<IFileService>();
            mockFileService.Setup(m => m.Exists(It.IsAny<string>())).Returns(true);
            mockFileService.SetupGet(m => m.AppDataDir).Returns(Environment.CurrentDirectory);
            var mockStream = new Mock<Stream>();
            mockFileService.Setup(m => m.Open(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>()))
                           .Returns(mockStream.Object);
            _subject.FileService = mockFileService.Object;

            try
            {
                _subject.Save(new Clipping());
                Assert.Fail("this should throw");
            }
            catch
            {
                mockStream.Verify(m => m.Close(), Times.Exactly(2));
            }
        }
    }
}
