namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using FluentAssertions;
    using Moq;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniApi.Resources.v1;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class GetRemoteConfigurationTests
    {
        private GetRemoteConfiguration _subject;

        private Mock<IOAuth2> _mockOAuth2;

        [SetUp]
        public void Setup()
        {
            var mockKernel = new MoqMockingKernel();

            _mockOAuth2 = mockKernel.GetMock<IOAuth2>();

            _subject = new GetRemoteConfiguration(_mockOAuth2.Object);
        }

        [Test]
        public void ExecutePayloadIsAnEmptyStringShouldReturnAResultWithStatusFailed()
        {
            _subject.Parameter = new DependencyParameter(string.Empty, string.Empty);

            _subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        }

//        [Test]
//        public void ExecutePayloadIsNonEmptyStringShouldCallActivationDataProviderGetActivationDataWithThePayload()
//        {
//            var createSubject = new Subject<Token>();
//            _subject.Parameter = new DependencyParameter(string.Empty, "42");
//            _mockOAuth2.Setup(m => m.Create(It.IsAny<string>())).Returns(createSubject);
//
//            var task = _subject.ExecuteAsync();
//            Thread.Sleep(100);
//            createSubject.OnNext(new Token());
//            Thread.Sleep(100);
//            task.Wait();
//
//            _mockOAuth2.Verify(m => m.Create("42"), Times.Once);
//        }

        //[Test]
        //public void ExecutePayloadIsARetryInfoObjectWithFailCountSmallerThanMaxFailCountAndEmptyTokenShouldReturnAResultWithStatusFailed()
        //{
        //    var subject = new GetRemoteConfiguration
        //    {
        //        ActivationTokens = _mockAuthorizationAPI.Object,
        //        Parameter = new DependencyParameter(string.Empty, new RetryInfo(string.Empty, GetRemoteConfiguration.MaxRetryCount - 1))
        //    };

        //    subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        //}

        //[Test]
        //public void ExecutePayloadIsARetryInfoObjectWithFailCountSmallerThanMaxFailCountAndNullTokenShouldReturnAResultWithStatusFailed()
        //{
        //    var subject = new GetRemoteConfiguration
        //    {
        //        ActivationTokens = _mockAuthorizationAPI.Object,
        //        Parameter = new DependencyParameter(string.Empty, new RetryInfo(null, GetRemoteConfiguration.MaxRetryCount - 1))
        //    };

        //    subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        //}

        //[Test]
        //public void ExecutePayloadIsARetryInfoObjectWithFailCountSmallerThanMaxFailCountAndNonEmptyTokenShouldCallActivationDataProviderGetActivationDataWithTheToken()
        //{
        //    var subject = new GetRemoteConfiguration
        //    {
        //        ActivationTokens = _mockAuthorizationAPI.Object,
        //        Parameter = new DependencyParameter(string.Empty, new RetryInfo("testToken", GetRemoteConfiguration.MaxRetryCount - 1))
        //    };

        //    subject.Execute();

        //    _mockAuthorizationAPI.Verify(x => x.Activate(_token), Times.Once());
        //}

        //[Test]
        //public void ExecutePayloadIsARetryInfoObjectWithFailCountEqualToMaxFailCountAndNonEmptyTokenShouldCallActivationDataProviderGetActivationDataWithTheToken()
        //{
        //    var subject = new GetRemoteConfiguration
        //    {
        //        ActivationTokens = _mockAuthorizationAPI.Object,
        //        Parameter = new DependencyParameter(string.Empty, new RetryInfo("testToken", GetRemoteConfiguration.MaxRetryCount))
        //    };

        //    subject.Execute();

        //    _mockAuthorizationAPI.Verify(x => x.Activate(_token), Times.Once());
        //}

        //[Test]
        //public void ExecutePayloadIsARetryInfoObjectWithFailCountGreaterThanMaxFailCountAndNonEmptyTokenShouldReturnAResultWithStatusFailed()
        //{
        //    var subject = new GetRemoteConfiguration()
        //                      {
        //                          ActivationTokens = _mockAuthorizationAPI.Object,
        //                          Parameter = new DependencyParameter(string.Empty, new RetryInfo("testToken", GetRemoteConfiguration.MaxRetryCount + 1))
        //                      };

        //    subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        //}

        //[Test]
        //public void ExecuteGetConfigurationReturnsEmptyActivationDataShouldReturnAResultWithStatusFailed()
        //{
        //    _subject.Execute();

        //    _subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        //}

        //[Test]
        //public void ExecuteGetConfigurationReturnsValidActivationDataObjectShouldReturnAResultWithStatusSuccessful()
        //{
        //    var activationData = new ActivationModel { Email = "test@email.com" };
        //    _mockAuthorizationAPI.Setup(u => u.Activate(It.IsAny<string>())).Returns(activationData);

        //    _subject.Execute();

        //    _subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.Successful);
        //}

        //[Test]
        //public void ExecuteGetConfigurationReturnsValidActivationDataObjectShouldReturnAResultWithDataContainingTheEmail()
        //{
        //    var activationData = new ActivationModel { Email = "test@email.com" };
        //    _mockAuthorizationAPI.Setup(u => u.Activate(It.IsAny<string>())).Returns(activationData);

        //    _subject.Execute();

        //    _subject.Execute().Data.Should().Be("test@email.com");
        //}

        //[Test]
        //public void ExecuteGetConfigurationReturnsActivationDataObjectWithCommnuicationErrorsShouldReturnAResultWithStatusCommunicationFailure()
        //{
        //    var activationData = new ActivationModel { CommunicationError = "error" };
        //    _mockAuthorizationAPI.Setup(u => u.Activate(It.IsAny<string>())).Returns(activationData);

        //    _subject.Execute();

        //    _subject.Execute().State.Should().Be(GetRemoteConfigurationStepStateEnum.CommunicationFailure);
        //}

        //[Test]
        //public void ExecuteGotConfigurationWithCommnuicationErrorAndFailCountLessThanMaxFailCountShouldReturnAResultWithTheCommunicationFailure()
        //{
        //    const string AuthorizationCode = "testToken";
        //    var retryInfo = new RetryInfo(AuthorizationCode, GetRemoteConfiguration.MaxRetryCount - 1);
        //    var subject = new GetRemoteConfiguration
        //    {
        //        ActivationTokens = _mockAuthorizationAPI.Object,
        //        Parameter = new DependencyParameter(string.Empty, retryInfo)
        //    };
        //    var activationData = new ActivationModel { CommunicationError = "error" };
        //    _mockAuthorizationAPI.Setup(u => u.Activate(It.Is<string>(s => s == AuthorizationCode))).Returns(activationData);

        //    var executeResult = subject.Execute();

        //    executeResult.Data.Should().BeOfType<RetryInfo>();
        //    ((RetryInfo)executeResult.Data).Error.Should().Be("error");
        //}

        //[Test]
        //public void ExecuteGotConfigurationWithCommnuicationErrorAndFailCountLessThanMaxFailCountShouldReturnAResultWithAIncrementedFailCount()
        //{
        //    const string AuthorizationCode = "testToken";
        //    var retryInfo = new RetryInfo(AuthorizationCode, GetRemoteConfiguration.MaxRetryCount - 1);
        //    var subject = new GetRemoteConfiguration
        //    {
        //        ActivationTokens = _mockAuthorizationAPI.Object,
        //        Parameter = new DependencyParameter(string.Empty, retryInfo)
        //    };
        //    var activationData = new ActivationModel { CommunicationError = "error" };
        //    _mockAuthorizationAPI.Setup(u => u.Activate(It.IsAny<string>())).Returns(activationData);

        //    var executeResult = subject.Execute();

        //    executeResult.Data.Should().BeOfType<RetryInfo>();
        //    ((RetryInfo)executeResult.Data).FailCount.Should().Be(GetRemoteConfiguration.MaxRetryCount);
        //}

        //[Test]
        //public void ExecuteGotConfigurationWithCommnuicationErrorAndFailCountLessThanMaxFailCountShouldReturnAResultWithTheGivenToken()
        //{
        //    const string AuthorizationCode = "testToken";
        //    var retryInfo = new RetryInfo(AuthorizationCode, GetRemoteConfiguration.MaxRetryCount - 1);
        //    var subject = new GetRemoteConfiguration
        //    {
        //        ActivationTokens = _mockAuthorizationAPI.Object,
        //        Parameter = new DependencyParameter(string.Empty, retryInfo)
        //    };
        //    var activationData = new ActivationModel { CommunicationError = "error" };
        //    _mockAuthorizationAPI.Setup(u => u.Activate(It.Is<string>(s => s == AuthorizationCode))).Returns(activationData);

        //    var executeResult = subject.Execute();

        //    executeResult.Data.Should().BeOfType<RetryInfo>();
        //    ((RetryInfo)executeResult.Data).AuthorizationCode.Should().Be(AuthorizationCode);
        //}

        //[Test]
        //public void ExecuteGotConfigurationWithCommnuicationErrorAndFailCountEqualToMaxFailCountShouldReturnAResultWithStatusFailed()
        //{
        //    const string AuthorizationCode = "testToken";
        //    var retryInfo = new RetryInfo(AuthorizationCode, GetRemoteConfiguration.MaxRetryCount);
        //    var subject = new GetRemoteConfiguration
        //                      {
        //                          ActivationTokens = _mockAuthorizationAPI.Object,
        //                          Parameter = new DependencyParameter(string.Empty, retryInfo)
        //                      };
        //    var activationData = new ActivationModel { CommunicationError = "error" };
        //    _mockAuthorizationAPI.Setup(u => u.Activate(AuthorizationCode)).Returns(activationData);

        //    var executeResult = subject.Execute();

        //    executeResult.State.Should().Be(GetRemoteConfigurationStepStateEnum.Failed);
        //}
    }
}