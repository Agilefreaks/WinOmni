namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class FixProxyConfigurationTests
    {
        private FixProxyConfiguration _subject;

        private Mock<IConfigurationService> _mockConfigurationService;

        private List<IProxyConfigurationDetector> _proxyConfigurationDetectors;

        [SetUp]
        public void Setup()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            _proxyConfigurationDetectors = new List<IProxyConfigurationDetector>();
            _subject = new FixProxyConfiguration(_mockConfigurationService.Object, _proxyConfigurationDetectors);
        }

        [Test]
        public void Execute_NoProxyConfigurationDetectorsAreGivenAndExistingConfigurationIsEmpty_ReturnsAFailedResult()
        {
            _mockConfigurationService.Setup(x => x.ProxyConfiguration).Returns(ProxyConfiguration.Empty());

            var executeResult = _subject.Execute().Wait();

            executeResult.State.Should().Be(SimpleStepStateEnum.Failed);
        }

        [Test]
        public void Execute_NoProxyConfigurationDetectorsAreGivenAndExistingConfigurationIsNotEmpty_SavesAnEmptyConfiguration()
        {
            _mockConfigurationService.Setup(x => x.ProxyConfiguration).Returns(new ProxyConfiguration { Address = "test" });

            _subject.Execute().Wait();

            _mockConfigurationService.Verify(x => x.SaveProxyConfiguration(ProxyConfiguration.Empty()));
        }

        [Test]
        public void Execute_NoProxyConfigurationDetectorsAreGivenAndExistingConfigurationIsNotEmpty_ReturnsASuccessResult()
        {
            _mockConfigurationService.Setup(x => x.ProxyConfiguration).Returns(new ProxyConfiguration { Address = "test" });

            var executeResult = _subject.Execute().Wait();

            executeResult.State.Should().Be(SimpleStepStateEnum.Successful);
        }

        [Test]
        public void Execute_AProxyConfigurationDetectorReturnsADifferentConfigurationThanTheExistingOne_SavesTheNewConfiguration()
        {
            var mockProxyConfigurationDetector = new Mock<IProxyConfigurationDetector>();
            var proxyConfiguration = new ProxyConfiguration { Address = "testB" };
            mockProxyConfigurationDetector.Setup(x => x.Detect()).Returns(proxyConfiguration);
            _mockConfigurationService.Setup(x => x.ProxyConfiguration)
                .Returns(new ProxyConfiguration { Address = "testA" });
            _proxyConfigurationDetectors.Add(mockProxyConfigurationDetector.Object);

            _subject.Execute().Wait();

            _mockConfigurationService.Verify(x => x.SaveProxyConfiguration(proxyConfiguration), Times.Once);
        }

        [Test]
        public void Execute_AProxyConfigurationDetectorReturnsADifferentConfigurationThanTheExistingOne_ReturnsASuccessResult()
        {
            var mockProxyConfigurationDetector = new Mock<IProxyConfigurationDetector>();
            mockProxyConfigurationDetector.Setup(x => x.Detect()).Returns(new ProxyConfiguration { Address = "testB" });
            _mockConfigurationService.Setup(x => x.ProxyConfiguration)
                .Returns(new ProxyConfiguration { Address = "testA" });
            _proxyConfigurationDetectors.Add(mockProxyConfigurationDetector.Object);

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
        public void Execute_AllProxyConfigurationDetectorsReturnNullAndProxyConfigurationIsNotEmpty_ReturnsASuccessResult()
        {
            var mockProxyConfigurationDetector = new Mock<IProxyConfigurationDetector>();
            mockProxyConfigurationDetector.Setup(x => x.Detect()).Returns((ProxyConfiguration?)null);
            var existingConfiguration = new ProxyConfiguration { Address = "test" };
            _mockConfigurationService.Setup(x => x.ProxyConfiguration).Returns(existingConfiguration);

            var executeResult = _subject.Execute().Wait();

            executeResult.State.Should().Be(SimpleStepStateEnum.Successful);
        }

        [Test]
        public void Execute_AllProxyConfigurationDetectorsReturnNullAndProxyConfigurationIsNotEmpty_SavesAnEmptyConfiguration()
        {
            var mockProxyConfigurationDetector = new Mock<IProxyConfigurationDetector>();
            mockProxyConfigurationDetector.Setup(x => x.Detect()).Returns((ProxyConfiguration?)null);
            var existingConfiguration = new ProxyConfiguration { Address = "test" };
            _mockConfigurationService.Setup(x => x.ProxyConfiguration).Returns(existingConfiguration);

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