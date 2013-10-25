namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniApi.Resources;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class GetRemoteConfigurationTests
    {
        private GetRemoteConfiguration _subject;

        private string _token;

        private Mock<IActivationTokens> _mockActivationTokensApiResource;

        [SetUp]
        public void Setup()
        {
            _mockActivationTokensApiResource = new Mock<IActivationTokens>();
            _mockActivationTokensApiResource.Setup(u => u.Activate(It.IsAny<string>())).Returns(new ActivationModel());
            _token = "testToken";
            _subject = new GetRemoteConfiguration
                                {
                                    ActivationTokens = _mockActivationTokensApiResource.Object,
                                    Parameter = new DependencyParameter(string.Empty, _token)
                                };
        }

        [Test]
        public void CtorAlwaysSetsTheOmniApi()
        {
            Assert.AreEqual(_subject.ActivationTokens, _mockActivationTokensApiResource.Object);
        }

        [Test]
        public void ExecutePayloadIsNullShouldReturnAResultWithStatusFailed()
        {
            var subject = new GetRemoteConfiguration
                              {
                                  ActivationTokens = _mockActivationTokensApiResource.Object,
                                  Parameter = new DependencyParameter(string.Empty, null)
                              };

            subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        }

        [Test]
        public void ExecutePayloadIsAnEmptyStringShouldReturnAResultWithStatusFailed()
        {
            var subject = new GetRemoteConfiguration
                              {
                                  ActivationTokens = _mockActivationTokensApiResource.Object,
                                  Parameter = new DependencyParameter(string.Empty, string.Empty)
                              };

            subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        }

        [Test]
        public void ExecutePayloadIsNonEmptyStringShouldCallActivationDataProviderGetActivationDataWithThePayload()
        {
            _subject.Execute();

            _mockActivationTokensApiResource.Verify(x => x.Activate(_token), Times.Once());
        }

        [Test]
        public void ExecutePayloadIsARetryInfoObjectWithFailCountSmallerThanMaxFailCountAndEmptyTokenShouldReturnAResultWithStatusFailed()
        {
            var subject = new GetRemoteConfiguration
            {
                ActivationTokens = _mockActivationTokensApiResource.Object,
                Parameter = new DependencyParameter(string.Empty, new RetryInfo(string.Empty, GetRemoteConfiguration.MaxRetryCount - 1))
            };

            subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        }

        [Test]
        public void ExecutePayloadIsARetryInfoObjectWithFailCountSmallerThanMaxFailCountAndNullTokenShouldReturnAResultWithStatusFailed()
        {
            var subject = new GetRemoteConfiguration
            {
                ActivationTokens = _mockActivationTokensApiResource.Object,
                Parameter = new DependencyParameter(string.Empty, new RetryInfo(null, GetRemoteConfiguration.MaxRetryCount - 1))
            };

            subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        }

        [Test]
        public void ExecutePayloadIsARetryInfoObjectWithFailCountSmallerThanMaxFailCountAndNonEmptyTokenShouldCallActivationDataProviderGetActivationDataWithTheToken()
        {
            var subject = new GetRemoteConfiguration
            {
                ActivationTokens = _mockActivationTokensApiResource.Object,
                Parameter = new DependencyParameter(string.Empty, new RetryInfo("testToken", GetRemoteConfiguration.MaxRetryCount - 1))
            };

            subject.Execute();

            _mockActivationTokensApiResource.Verify(x => x.Activate(_token), Times.Once());
        }

        [Test]
        public void ExecutePayloadIsARetryInfoObjectWithFailCountEqualToMaxFailCountAndNonEmptyTokenShouldCallActivationDataProviderGetActivationDataWithTheToken()
        {
            var subject = new GetRemoteConfiguration
            {
                ActivationTokens = _mockActivationTokensApiResource.Object,
                Parameter = new DependencyParameter(string.Empty, new RetryInfo("testToken", GetRemoteConfiguration.MaxRetryCount))
            };

            subject.Execute();

            _mockActivationTokensApiResource.Verify(x => x.Activate(_token), Times.Once());
        }

        [Test]
        public void ExecutePayloadIsARetryInfoObjectWithFailCountGreaterThanMaxFailCountAndNonEmptyTokenShouldReturnAResultWithStatusFailed()
        {
            var subject = new GetRemoteConfiguration()
                              {
                                  ActivationTokens = _mockActivationTokensApiResource.Object,
                                  Parameter = new DependencyParameter(string.Empty, new RetryInfo("testToken", GetRemoteConfiguration.MaxRetryCount + 1))
                              };

            subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        }

        [Test]
        public void ExecuteGetConfigurationReturnsEmptyActivationDataShouldReturnAResultWithStatusFailed()
        {
            _subject.Execute();

            _subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        }

        [Test]
        public void ExecuteGetConfigurationReturnsValidActivationDataObjectShouldReturnAResultWithStatusSuccessful()
        {
            var activationData = new ActivationModel { Email = "test@email.com" };
            _mockActivationTokensApiResource.Setup(u => u.Activate(It.IsAny<string>())).Returns(activationData);

            _subject.Execute();

            _subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Successful);
        }

        [Test]
        public void ExecuteGetConfigurationReturnsValidActivationDataObjectShouldReturnAResultWithDataContainingTheEmail()
        {
            var activationData = new ActivationModel { Email = "test@email.com" };
            _mockActivationTokensApiResource.Setup(u => u.Activate(It.IsAny<string>())).Returns(activationData);

            _subject.Execute();

            _subject.Execute().Data.Should().Be("test@email.com");
        }

        [Test]
        public void ExecuteGetConfigurationReturnsActivationDataObjectWithCommnuicationErrorsShouldReturnAResultWithStatusCommunicationFailure()
        {
            var activationData = new ActivationModel { CommunicationError = "error" };
            _mockActivationTokensApiResource.Setup(u => u.Activate(It.IsAny<string>())).Returns(activationData);

            _subject.Execute();

            _subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.CommunicationFailure);
        }

        [Test]
        public void ExecuteGotConfigurationWithCommnuicationErrorAndFailCountLessThanMaxFailCountShouldReturnAResultWithTheCommunicationFailure()
        {
            const string Token = "testToken";
            var retryInfo = new RetryInfo(Token, GetRemoteConfiguration.MaxRetryCount - 1);
            var subject = new GetRemoteConfiguration
            {
                ActivationTokens = _mockActivationTokensApiResource.Object,
                Parameter = new DependencyParameter(string.Empty, retryInfo)
            };
            var activationData = new ActivationModel { CommunicationError = "error" };
            _mockActivationTokensApiResource.Setup(u => u.Activate(It.Is<string>(s => s == Token))).Returns(activationData);

            var executeResult = subject.Execute();

            executeResult.Data.Should().BeOfType<RetryInfo>();
            ((RetryInfo)executeResult.Data).Error.Should().Be("error");
        }

        [Test]
        public void ExecuteGotConfigurationWithCommnuicationErrorAndFailCountLessThanMaxFailCountShouldReturnAResultWithAIncrementedFailCount()
        {
            const string Token = "testToken";
            var retryInfo = new RetryInfo(Token, GetRemoteConfiguration.MaxRetryCount - 1);
            var subject = new GetRemoteConfiguration
            {
                ActivationTokens = _mockActivationTokensApiResource.Object,
                Parameter = new DependencyParameter(string.Empty, retryInfo)
            };
            var activationData = new ActivationModel { CommunicationError = "error" };
            _mockActivationTokensApiResource.Setup(u => u.Activate(It.IsAny<string>())).Returns(activationData);

            var executeResult = subject.Execute();

            executeResult.Data.Should().BeOfType<RetryInfo>();
            ((RetryInfo)executeResult.Data).FailCount.Should().Be(GetRemoteConfiguration.MaxRetryCount);
        }

        [Test]
        public void ExecuteGotConfigurationWithCommnuicationErrorAndFailCountLessThanMaxFailCountShouldReturnAResultWithTheGivenToken()
        {
            const string Token = "testToken";
            var retryInfo = new RetryInfo(Token, GetRemoteConfiguration.MaxRetryCount - 1);
            var subject = new GetRemoteConfiguration
            {
                ActivationTokens = _mockActivationTokensApiResource.Object,
                Parameter = new DependencyParameter(string.Empty, retryInfo)
            };
            var activationData = new ActivationModel { CommunicationError = "error" };
            _mockActivationTokensApiResource.Setup(u => u.Activate(It.Is<string>(s => s == Token))).Returns(activationData);

            var executeResult = subject.Execute();

            executeResult.Data.Should().BeOfType<RetryInfo>();
            ((RetryInfo)executeResult.Data).Token.Should().Be(Token);
        }

        [Test]
        public void ExecuteGotConfigurationWithCommnuicationErrorAndFailCountEqualToMaxFailCountShouldReturnAResultWithStatusFailed()
        {
            const string Token = "testToken";
            var retryInfo = new RetryInfo(Token, GetRemoteConfiguration.MaxRetryCount);
            var subject = new GetRemoteConfiguration
                              {
                                  ActivationTokens = _mockActivationTokensApiResource.Object,
                                  Parameter = new DependencyParameter(string.Empty, retryInfo)
                              };
            var activationData = new ActivationModel { CommunicationError = "error" };
            _mockActivationTokensApiResource.Setup(u => u.Activate(Token)).Returns(activationData);

            var executeResult = subject.Execute();

            executeResult.State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        }
    }
}