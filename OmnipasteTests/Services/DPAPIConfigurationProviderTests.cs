namespace OmnipasteTests.Services
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Services;

    [TestFixture]
    public class DPAPIConfigurationProviderTests
    {
        private DPAPIConfigurationProvider _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new DPAPIConfigurationProvider();
        }

        [Test]
        public void SettingsFolder_Should_ReturnCorrectPath()
        {
            _subject.SettingsFolder.Should().EndWith("\\Omnipaste-debug");
        }
    }
}