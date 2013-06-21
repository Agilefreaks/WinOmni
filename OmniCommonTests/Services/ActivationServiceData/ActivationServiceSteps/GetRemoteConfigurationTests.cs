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
            var subject = new GetRemoteConfiguration(_mockActivationDataProvider.Object, null);

            subject.Execute().State.Should().Be(GetConfigurationStepStateEnum.Failed);
        }

        [Test]
        public void Execute_PayloadIsAnEmptyString_ShouldReturnAResultWithStatusFailed()
        {
            var subject = new GetRemoteConfiguration(_mockActivationDataProvider.Object, string.Empty);

            subject.Execute().State.Should().Be(GetConfigurationStepStateEnum.Failed);
        }

        [Test]
        public void Execute_PayloadIsNonEmptyString_ShouldCallActivationDataProviderGetActivationDataWithThePayload()
        {
            _subject.Execute();

            _mockActivationDataProvider.Verify(x => x.GetActivationData(_token), Times.Once());
        }

        [Test]
        public void Execute_GetConfigurationReturnsEmptyActivationData_ShouldReturnAResultWithStatusFailed()
        {
            _mockActivationDataProvider.Setup(x => x.GetActivationData(_token)).Returns(new ActivationData());

            _subject.Execute();

            _subject.Execute().State.Should().Be(GetConfigurationStepStateEnum.Failed);
        }

        [Test]
        public void Execute_GetConfigurationReturnsValidActivationDataObject_ShouldReturnAResultWithStatusSuccessful()
        {
            var activationData = new ActivationData { Email = "test@email.com" };
            _mockActivationDataProvider.Setup(x => x.GetActivationData(_token)).Returns(activationData);

            _subject.Execute();

            _subject.Execute().State.Should().Be(GetConfigurationStepStateEnum.Successful);
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

            _subject.Execute().State.Should().Be(GetConfigurationStepStateEnum.CommunicationFailure);
        }

        [Test]
        public void Execute_GetConfigurationReturnsActivationDataObjectWithCommnuicationErrors_ShouldReturnAResultWithDataContainingTheCommunicationFailure()
        {
            var activationData = new ActivationData { CommunicationError = "error" };
            _mockActivationDataProvider.Setup(x => x.GetActivationData(_token)).Returns(activationData);

            _subject.Execute();

            _subject.Execute().Data.Should().Be("error");
        }
    }
}