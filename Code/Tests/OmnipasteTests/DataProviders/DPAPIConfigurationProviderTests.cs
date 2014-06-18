namespace OmnipasteTests.DataProviders
{
    using System.IO;
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.DataProviders;

    public class DPAPIConfigurationProviderTests
    {
        #region Fields

        private DPAPIConfigurationProvider _subject;

        #endregion

        #region Public Methods and Operators
        
        [SetUp]
        public void Setup()
        {
            _subject = new DPAPIConfigurationProvider
                           {
                               SettingsFolder = Path.GetTempPath(),
                               SettingsFileName = Path.GetRandomFileName()
                           };
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(_subject.FullSettingsFilePath);
        }

        [Test]
        public void GetValue_WhenValueWasSet_WillReturnThatValue()
        {
            _subject.SetValue("key", "value");
            _subject.GetValue("key").Should().Be("value");
        }

        [Test]
        public void GetValue_WhenValueWasNotSet_WillReturnNull()
        {
            _subject.GetValue("other").Should().BeEmpty();
        }

        [Test]
        public void GenericGetValue_WhenNoValueWasSet_WillReturnDefault()
        {
            _subject.GetValue("someFlag", true).Should().BeTrue();
        }

        [Test]
        public void GenericGetValue_WhenNoValueWasSet_WillReturnDefaultForString()
        {
            _subject.GetValue("someString", "default value").Should().Be("default value");
        }

        [Test]
        public void SetValue_WillSetValues()
        {
            _subject.SetValue("token", "token value");
            _subject.SetValue("type", "type value");

            _subject.GetValue("token").Should().Be("token value");
            _subject.GetValue("type").Should().Be("type value");
        }

        [Test]
        public void SetValue_WillUpdateValues()
        {
            _subject.SetValue("token", "token value");

            _subject.GetValue("token").Should().Be("token value");

            _subject.SetValue("token", "token value update");

            _subject.GetValue("token").Should().Be("token value update");
        }

        #endregion
    }
}