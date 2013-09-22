namespace Omnipaste.OmniClipboard.Infrastructure.Tests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.DataProviders;
    using Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class GetTokenFromDeploymentUriTests
    {
        private GetTokenFromDeploymentUri _subject;

        private Mock<IApplicationDeploymentInfoProvider> _mockApplicationDeploymentInfoProvider;

        [SetUp]
        public void Setup()
        {
            this._mockApplicationDeploymentInfoProvider = new Mock<IApplicationDeploymentInfoProvider>();
            this._subject = new GetTokenFromDeploymentUri(this._mockApplicationDeploymentInfoProvider.Object);
        }

        [Test]
        public void Execute_ProviderDoesNotHasValidActivationUri_ShouldReturnAResultWithStatusFailed()
        {
            this._mockApplicationDeploymentInfoProvider.Setup(x => x.HasValidActivationUri).Returns(false);

            this._subject.Execute().State.Should().Be(SimpleStepStateEnum.Failed);
        }

        [Test]
        public void Execute_ProviderHasValidActivationUriButTheUriDoesNotHaveATokenParameter_ShouldReturnAResultWithStatusFailed()
        {
            this._mockApplicationDeploymentInfoProvider.Setup(x => x.HasValidActivationUri).Returns(true);
            this._mockApplicationDeploymentInfoProvider.Setup(x => x.ActivationUri).Returns(new Uri("http://www.google.com"));

            this._subject.Execute().State.Should().Be(SimpleStepStateEnum.Failed);
        }

        [Test]
        public void Execute_ProviderHasValidActivationUriAndTheUriHasATokenParameter_ShouldReturnAResultWithStatusSuccessfulAndTheToken()
        {
            this._mockApplicationDeploymentInfoProvider.Setup(x => x.HasValidActivationUri).Returns(true);
            this._mockApplicationDeploymentInfoProvider.Setup(x => x.ActivationUri).Returns(new Uri("http://www.google.com?token=testToken"));

            var result = this._subject.Execute();
            result.State.Should().Be(SimpleStepStateEnum.Successful);
            result.Data.Should().Be("testToken");
        }
    }
}