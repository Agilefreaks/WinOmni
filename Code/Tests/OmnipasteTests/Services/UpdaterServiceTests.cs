namespace OmnipasteTests.Services
{
    using System;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using Omnipaste.Services;

    [TestFixture]
    public class UpdaterServiceTests
    {
        private UpdaterService _subject;

        private Mock<ISystemIdleService> _mockSystemIdleService;

        private Mock<IConfigurationService> _mockConfigurationService;

        private Func<UpdaterService> _createInstance;

        [SetUp]
        public void Setup()
        {
            _mockSystemIdleService = new Mock<ISystemIdleService>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _createInstance = () => new UpdaterService(_mockSystemIdleService.Object, _mockConfigurationService.Object);
            _subject = _createInstance();
        }

        [Test]
        public void UpdateCheckInterval_ConfigurationHasNoUpdateInterval_IsSetByTheConstructorToTheDefault()
        {
            _mockConfigurationService.SetupGet(x => x[ConfigurationProperties.UpdateInterval]).Returns((string)null);

            _createInstance().UpdateCheckInterval.Should().Be(TimeSpan.FromMinutes(60));
        }

        [Test]
        public void UpdateCheckInterval_ConfigurationHasAnInvalidUpdateInterval_IsSetByTheConstructorToTheDefault()
        {
            _mockConfigurationService.SetupGet(x => x[ConfigurationProperties.UpdateInterval]).Returns("test");

            _createInstance().UpdateCheckInterval.Should().Be(TimeSpan.FromMinutes(60));
        }

        [Test]
        public void UpdateCheckInterval_ConfigurationHasAValidUpdateInterval_IsSetByTheConstructorToTheGivenSettingValue()
        {
            _mockConfigurationService.SetupGet(x => x[ConfigurationProperties.UpdateInterval]).Returns("2");

            _createInstance().UpdateCheckInterval.Should().Be(TimeSpan.FromMinutes(2));
        }
    }
}