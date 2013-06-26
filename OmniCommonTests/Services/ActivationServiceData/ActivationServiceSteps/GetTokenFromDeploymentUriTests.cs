namespace OmniCommonTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.DataProviders;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class GetTokenFromDeploymentUriTests
    {
        private GetTokenFromDeploymentUri _subject;

        private Mock<IApplicationDeploymentInfoProvider> _mockApplicationDeploymentInfoProvider;

        [SetUp]
        public void Setup()
        {
            _mockApplicationDeploymentInfoProvider = new Mock<IApplicationDeploymentInfoProvider>();
            _subject = new GetTokenFromDeploymentUri(_mockApplicationDeploymentInfoProvider.Object);
        }

        [Test]
        public void Execute_ProviderDoesNotHasValidActivationUri_ShouldReturnAResultWithStatusFailed()
        {
            _mockApplicationDeploymentInfoProvider.Setup(x => x.HasValidActivationUri).Returns(false);

            _subject.Execute().State.Should().Be(SimpleStepStateEnum.Failed);
        }

        [Test]
        public void Execute_ProviderHasValidActivationUriButTheUriDoesNotHaveATokenParameter_ShouldReturnAResultWithStatusFailed()
        {
            _mockApplicationDeploymentInfoProvider.Setup(x => x.HasValidActivationUri).Returns(true);
            _mockApplicationDeploymentInfoProvider.Setup(x => x.ActivationUri).Returns(new Uri("http://www.google.com"));

            _subject.Execute().State.Should().Be(SimpleStepStateEnum.Failed);
        }

        [Test]
        public void Execute_ProviderHasValidActivationUriAndTheUriHasATokenParameter_ShouldReturnAResultWithStatusSuccessfulAndTheToken()
        {
            _mockApplicationDeploymentInfoProvider.Setup(x => x.HasValidActivationUri).Returns(true);
            _mockApplicationDeploymentInfoProvider.Setup(x => x.ActivationUri).Returns(new Uri("http://www.google.com?token=testToken"));

            var result = _subject.Execute();
            result.State.Should().Be(SimpleStepStateEnum.Successful);
            result.Data.Should().Be("testToken");
        }
    }
}