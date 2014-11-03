namespace OmnipasteTests.Services
{
    using System;
    using System.Net;
    using FluentAssertions;
    using Moq;
    using NAppUpdate.Framework;
    using NAppUpdate.Framework.Sources;
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

        private Mock<IWebProxyFactory> _mockWebProxyFactory;

        [SetUp]
        public void Setup()
        {
            _mockSystemIdleService = new Mock<ISystemIdleService>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockWebProxyFactory = new Mock<IWebProxyFactory>();
            _createInstance =
                () =>
                new UpdaterService(
                    _mockSystemIdleService.Object,
                    _mockConfigurationService.Object,
                    _mockWebProxyFactory.Object);
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

        [Test]
        public void Ctor_Always_SetsTheProxyReturnedByTheProxyFactoryOnTheUpdateSource()
        {
            var mockProxy = new Mock<IWebProxy>();
            _mockWebProxyFactory.Setup(x => x.CreateFromAppConfiguration()).Returns(mockProxy.Object);

            _createInstance();

            ((SimpleWebSource)UpdateManager.Instance.UpdateSource).Proxy.Should().Be(mockProxy.Object);
        }
    }
}