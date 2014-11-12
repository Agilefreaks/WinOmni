namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using Omnipaste.Services;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class FixProxyConfigurationTests
    {
        private FixProxyConfiguration _subject;

        private Mock<IConfigurationService> _mockConfigurationService;

        private List<IProxyConfigurationDetector> _proxyConfigurationDetectors;

        private Mock<INetworkService> _mockNetworkService;

        [SetUp]
        public void Setup()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            _proxyConfigurationDetectors = new List<IProxyConfigurationDetector>();
            _mockNetworkService = new Mock<INetworkService>();
            _subject = new FixProxyConfiguration(
                _mockConfigurationService.Object,
                _proxyConfigurationDetectors,
                _mockNetworkService.Object);
        }

        [Test]
        public void Execute_NoProxyConfigurationDetectorsAreGivenAndPingFailsWithEmptyConfiguration_ReturnsAFailedResult()
        {
            _mockNetworkService.Setup(x => x.CanPingHome(ProxyConfiguration.Empty())).Returns(false);
            var executeResult = _subject.Execute().Wait();

            executeResult.State.Should().Be(SimpleStepStateEnum.Failed);
        }

        [Test]
        public void Execute_NoProxyConfigurationDetectorsAreGivenAndExistingConfigurationIsNotEmptyAndCanPingWithEmptyConfiguration_SavesAnEmptyConfiguration()
        {
            _mockNetworkService.Setup(x => x.CanPingHome(ProxyConfiguration.Empty())).Returns(true);
            _mockConfigurationService.Setup(x => x.ProxyConfiguration).Returns(new ProxyConfiguration { Address = "test" });

            _subject.Execute().Wait();

            _mockConfigurationService.Verify(x => x.SaveProxyConfiguration(ProxyConfiguration.Empty()));
        }

        [Test]
        public void Execute_NoProxyConfigurationDetectorsAreGivenAndExistingConfigurationIsNotEmptyAndCanPingWithEmptyConfiguration_ReturnsASuccessResult()
        {
            _mockNetworkService.Setup(x => x.CanPingHome(ProxyConfiguration.Empty())).Returns(true);
            _mockConfigurationService.Setup(x => x.ProxyConfiguration).Returns(new ProxyConfiguration { Address = "test" });

            var executeResult = _subject.Execute().Wait();

            executeResult.State.Should().Be(SimpleStepStateEnum.Successful);
        }

        [Test]
        public void Execute_AProxyConfigurationDetectorReturnsADifferentConfigurationThanTheExistingOneAndCanPingWithIt_SavesTheNewConfiguration()
        {
            var mockProxyConfigurationDetector = new Mock<IProxyConfigurationDetector>();
            var proxyConfiguration = new ProxyConfiguration { Address = "testB" };
            mockProxyConfigurationDetector.Setup(x => x.Detect()).Returns(proxyConfiguration);
            _mockConfigurationService.Setup(x => x.ProxyConfiguration)
                .Returns(new ProxyConfiguration { Address = "testA" });
            _proxyConfigurationDetectors.Add(mockProxyConfigurationDetector.Object);
            _mockNetworkService.Setup(x => x.CanPingHome(proxyConfiguration)).Returns(true);

            _subject.Execute().Wait();

            _mockConfigurationService.Verify(x => x.SaveProxyConfiguration(proxyConfiguration), Times.Once);
        }

        [Test]
        public void Execute_AProxyConfigurationDetectorReturnsADifferentConfigurationThanTheExistingOneAndCanPingWithIt_ReturnsASuccessResult()
        {
            var mockProxyConfigurationDetector = new Mock<IProxyConfigurationDetector>();
            mockProxyConfigurationDetector.Setup(x => x.Detect()).Returns(new ProxyConfiguration { Address = "testB" });
            _mockConfigurationService.Setup(x => x.ProxyConfiguration)
                .Returns(new ProxyConfiguration { Address = "testA" });
            _proxyConfigurationDetectors.Add(mockProxyConfigurationDetector.Object);
            _mockNetworkService.Setup(x => x.CanPingHome(It.IsAny<ProxyConfiguration>())).Returns(true);

            var executeResult = _subject.Execute().Wait();

            executeResult.State.Should().Be(SimpleStepStateEnum.Successful);
        }

        [Test]
        public void Execute_AllProxyConfigurationDetectorsReturnTheSameConfigurationsAsTheExistingOne_ReturnsAFailedResult()
        {
            var mockProxyConfigurationDetector = new Mock<IProxyConfigurationDetector>();
            mockProxyConfigurationDetector.Setup(x => x.Detect()).Returns(new ProxyConfiguration { Address = "testA" });
            _mockConfigurationService.Setup(x => x.ProxyConfiguration)
                .Returns(new ProxyConfiguration { Address = "testA" });
            _proxyConfigurationDetectors.Add(mockProxyConfigurationDetector.Object);

            var executeResult = _subject.Execute().Wait();

            executeResult.State.Should().Be(SimpleStepStateEnum.Failed);
        }

        [Test]
        public void Execute_AllProxyConfigurationDetectorsReturnConfigurationsForWhichPingFails_ReturnsAFailedResult()
        {
            var mockProxyConfigurationDetector = new Mock<IProxyConfigurationDetector>();
            mockProxyConfigurationDetector.Setup(x => x.Detect()).Returns(new ProxyConfiguration());
            _proxyConfigurationDetectors.Add(mockProxyConfigurationDetector.Object);
            _mockNetworkService.Setup(x => x.CanPingHome(It.IsAny<ProxyConfiguration?>())).Returns(false);

            var executeResult = _subject.Execute().Wait();

            executeResult.State.Should().Be(SimpleStepStateEnum.Failed);
        }

        [Test]
        public void Execute_AllProxyConfigurationDetectorsReturnNullAndProxyConfigurationIsNotEmptyAndCanPingWithEmptyConfiguration_ReturnsASuccessResult()
        {
            var mockProxyConfigurationDetector = new Mock<IProxyConfigurationDetector>();
            mockProxyConfigurationDetector.Setup(x => x.Detect()).Returns((ProxyConfiguration?)null);
            var existingConfiguration = new ProxyConfiguration { Address = "test" };
            _mockConfigurationService.Setup(x => x.ProxyConfiguration).Returns(existingConfiguration);
            _mockNetworkService.Setup(x => x.CanPingHome(ProxyConfiguration.Empty())).Returns(true);

            var executeResult = _subject.Execute().Wait();

            executeResult.State.Should().Be(SimpleStepStateEnum.Successful);
        }

        [Test]
        public void Execute_AllProxyConfigurationDetectorsReturnNullAndProxyConfigurationIsNotEmptyAndCanPingWithEmptyConfiguration_SavesAnEmptyConfiguration()
        {
            var mockProxyConfigurationDetector = new Mock<IProxyConfigurationDetector>();
            mockProxyConfigurationDetector.Setup(x => x.Detect()).Returns((ProxyConfiguration?)null);
            var existingConfiguration = new ProxyConfiguration { Address = "test" };
            _mockConfigurationService.Setup(x => x.ProxyConfiguration).Returns(existingConfiguration);
            _mockNetworkService.Setup(x => x.CanPingHome(ProxyConfiguration.Empty())).Returns(true);

            _subject.Execute().Wait();

            _mockConfigurationService.Verify(x => x.SaveProxyConfiguration(ProxyConfiguration.Empty()));
        }

        [Test]
        public void Execute_AllProxyConfigurationDetectorsReturnNullAndProxyConfigurationIsEmpty_ReturnsAFailedResult()
        {
            var mockProxyConfigurationDetector = new Mock<IProxyConfigurationDetector>();
            mockProxyConfigurationDetector.Setup(x => x.Detect()).Returns((ProxyConfiguration?)null);
            _mockConfigurationService.Setup(x => x.ProxyConfiguration).Returns(ProxyConfiguration.Empty);

            var executeResult = _subject.Execute().Wait();

            executeResult.State.Should().Be(SimpleStepStateEnum.Failed);
        }
    }
}