namespace OmniCommonTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.DataProviders;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class GetRemoteConfigurationTests
    {
        private GetRemoteConfiguration _subject;

        private Mock<IActivationDataProvider> _mockActivationDataProvider;

        private string _token;

        [SetUp]
        public void Setup()
        {
            _mockActivationDataProvider = new Mock<IActivationDataProvider>();
            _token = "testToken";
            _subject = new GetRemoteConfiguration(_mockActivationDataProvider.Object, _token);
            _mockActivationDataProvider.Setup(x => x.GetActivationData(It.IsAny<string>()))
                                       .Returns(new ActivationData());
        }

        [Test]
        public void Execute_PayloadIsNull_ShouldReturnAResultWithStatusFailed()
        {
            var subject = new GetRemoteConfiguration(_mockActivationDataProvider.Object, (string)null);

            subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        }

        [Test]
        public void Execute_PayloadIsAnEmptyString_ShouldReturnAResultWithStatusFailed()
        {
            var subject = new GetRemoteConfiguration(_mockActivationDataProvider.Object, string.Empty);

            subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        }

        [Test]
        public void Execute_PayloadIsNonEmptyString_ShouldCallActivationDataProviderGetActivationDataWithThePayload()
        {
            _subject.Execute();

            _mockActivationDataProvider.Verify(x => x.GetActivationData(_token), Times.Once());
        }

        [Test]
        public void Execute_PayloadIsARetryInfoObjectWithFailCountSmallerThanMaxFailCountAndEmptyToken_ShouldReturnAResultWithStatusFailed()
        {
            var subject = new GetRemoteConfiguration(
                _mockActivationDataProvider.Object,
                new RetryInfo(string.Empty, GetRemoteConfiguration.MaxRetryCount - 1));

            subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        }

        [Test]
        public void Execute_PayloadIsARetryInfoObjectWithFailCountSmallerThanMaxFailCountAndNullToken_ShouldReturnAResultWithStatusFailed()
        {
            var subject = new GetRemoteConfiguration(
                _mockActivationDataProvider.Object,
                new RetryInfo(null, GetRemoteConfiguration.MaxRetryCount - 1));

            subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        }

        [Test]
        public void Execute_PayloadIsARetryInfoObjectWithFailCountSmallerThanMaxFailCountAndNonEmptyToken_ShouldCallActivationDataProviderGetActivationDataWithTheToken()
        {
            var subject = new GetRemoteConfiguration(
                _mockActivationDataProvider.Object,
                new RetryInfo("testToken", GetRemoteConfiguration.MaxRetryCount - 1));

            subject.Execute();

            _mockActivationDataProvider.Verify(x => x.GetActivationData(_token), Times.Once());
        }

        [Test]
        public void Execute_PayloadIsARetryInfoObjectWithFailCountEqualToMaxFailCountAndNonEmptyToken_ShouldCallActivationDataProviderGetActivationDataWithTheToken()
        {
            var subject = new GetRemoteConfiguration(
                _mockActivationDataProvider.Object,
                new RetryInfo("testToken", GetRemoteConfiguration.MaxRetryCount));

            subject.Execute();

            _mockActivationDataProvider.Verify(x => x.GetActivationData(_token), Times.Once());
        }

        [Test]
        public void Execute_PayloadIsARetryInfoObjectWithFailCountGreaterThanMaxFailCountAndNonEmptyToken_ShouldReturnAResultWithStatusFailed()
        {
            var subject = new GetRemoteConfiguration(
                _mockActivationDataProvider.Object,
                new RetryInfo("testToken", GetRemoteConfiguration.MaxRetryCount + 1));

            subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        }

        [Test]
        public void Execute_GetConfigurationReturnsEmptyActivationData_ShouldReturnAResultWithStatusFailed()
        {
            _mockActivationDataProvider.Setup(x => x.GetActivationData(_token)).Returns(new ActivationData());

            _subject.Execute();

            _subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        }

        [Test]
        public void Execute_GetConfigurationReturnsValidActivationDataObject_ShouldReturnAResultWithStatusSuccessful()
        {
            var activationData = new ActivationData { Email = "test@email.com" };
            _mockActivationDataProvider.Setup(x => x.GetActivationData(_token)).Returns(activationData);

            _subject.Execute();

            _subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Successful);
        }

        [Test]
        public void Execute_GetConfigurationReturnsValidActivationDataObject_ShouldReturnAResultWithDataContainingTheEmail()
        {
            var activationData = new ActivationData { Email = "test@email.com" };
            _mockActivationDataProvider.Setup(x => x.GetActivationData(_token)).Returns(activationData);

            _subject.Execute();

            _subject.Execute().Data.Should().Be("test@email.com");
        }

        [Test]
        public void Execute_GetConfigurationReturnsActivationDataObjectWithCommnuicationErrors_ShouldReturnAResultWithStatusCommunicationFailure()
        {
            var activationData = new ActivationData { CommunicationError = "error" };
            _mockActivationDataProvider.Setup(x => x.GetActivationData(_token)).Returns(activationData);

            _subject.Execute();

            _subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.CommunicationFailure);
        }

        [Test]
        public void Execute_GotConfigurationWithCommnuicationErrorAndFailCountLessThanMaxFailCount_ShouldReturnAResultWithTheCommunicationFailure()
        {
            const string Token = "testToken";
            var retryInfo = new RetryInfo(Token, GetRemoteConfiguration.MaxRetryCount - 1);
            var subject = new GetRemoteConfiguration(_mockActivationDataProvider.Object, retryInfo);
            var activationData = new ActivationData { CommunicationError = "error" };
            _mockActivationDataProvider.Setup(x => x.GetActivationData(Token)).Returns(activationData);

            var executeResult = subject.Execute();

            executeResult.Data.Should().BeOfType<RetryInfo>();
            ((RetryInfo)executeResult.Data).Error.Should().Be("error");
        }

        [Test]
        public void Execute_GotConfigurationWithCommnuicationErrorAndFailCountLessThanMaxFailCount_ShouldReturnAResultWithAIncrementedFailCount()
        {
            const string Token = "testToken";
            var retryInfo = new RetryInfo(Token, GetRemoteConfiguration.MaxRetryCount - 1);
            var subject = new GetRemoteConfiguration(_mockActivationDataProvider.Object, retryInfo);
            var activationData = new ActivationData { CommunicationError = "error" };
            _mockActivationDataProvider.Setup(x => x.GetActivationData(Token)).Returns(activationData);

            var executeResult = subject.Execute();

            executeResult.Data.Should().BeOfType<RetryInfo>();
            ((RetryInfo)executeResult.Data).FailCount.Should().Be(GetRemoteConfiguration.MaxRetryCount);
        }

        [Test]
        public void Execute_GotConfigurationWithCommnuicationErrorAndFailCountLessThanMaxFailCount_ShouldReturnAResultWithTheGivenToken()
        {
            const string Token = "testToken";
            var retryInfo = new RetryInfo(Token, GetRemoteConfiguration.MaxRetryCount - 1);
            var subject = new GetRemoteConfiguration(_mockActivationDataProvider.Object, retryInfo);
            var activationData = new ActivationData { CommunicationError = "error" };
            _mockActivationDataProvider.Setup(x => x.GetActivationData(Token)).Returns(activationData);

            var executeResult = subject.Execute();

            executeResult.Data.Should().BeOfType<RetryInfo>();
            ((RetryInfo)executeResult.Data).Token.Should().Be(Token);
        }

        [Test]
        public void Execute_GotConfigurationWithCommnuicationErrorAndFailCountEqualToMaxFailCount_ShouldReturnAResultWithStatusFailed()
        {
            const string Token = "testToken";
            var retryInfo = new RetryInfo(Token, GetRemoteConfiguration.MaxRetryCount);
            var subject = new GetRemoteConfiguration(_mockActivationDataProvider.Object, retryInfo);
            var activationData = new ActivationData { CommunicationError = "error" };
            _mockActivationDataProvider.Setup(x => x.GetActivationData(Token)).Returns(activationData);

            var executeResult = subject.Execute();

            executeResult.State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        }
    }
}