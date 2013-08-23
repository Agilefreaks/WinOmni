namespace OmniCommonTests.Services
{
    using OmniCommon;
    using System.Configuration;
    using System.IO;
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Services;

    [TestFixture]
    public class FileServiceTest
    {
        private FileService _subject;

        [SetUp]
        public void SetUp()
        {
            _subject = new FileService();
        }

        [Test]
        public void AppDataDir_Always_ReturnsAppDataPath()
        {
            _subject.AppDataDir.Should()
                    .Be(
                        Path.Combine(
                            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                         AppInfo.PublisherName), ConfigurationManager.AppSettings["appName"]));
        }
    }
}
