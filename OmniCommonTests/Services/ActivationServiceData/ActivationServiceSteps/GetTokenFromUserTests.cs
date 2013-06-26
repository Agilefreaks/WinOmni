namespace OmniCommonTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.EventAggregatorMessages;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class GetTokenFromUserTests
    {
        private GetTokenFromUser _subject;

        private Mock<IEventAggregator> _mockEventAggregator;

        private TokenRequestResultMessage _tokenRequestResultMessage;

        [SetUp]
        public void Setup()
        {
            _mockEventAggregator = new Mock<IEventAggregator>();
            _subject = new GetTokenFromUser(_mockEventAggregator.Object);
            this._tokenRequestResultMessage =
                new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Successful, "test");

            _mockEventAggregator.Setup(x => x.Publish(It.IsAny<GetTokenFromUserMessage>()))
                                .Callback(() => _subject.Handle(this._tokenRequestResultMessage));
        }

        [Test]
        public void Ctor_Always_ShouldCallEventAggregatorSubscribeWithSelf()
        {
            _mockEventAggregator.Verify(x => x.Subscribe(_subject));
        }

        [Test]
        public void Execute_Always_ShouldCallEventAggregatorPublishWithTheGetTokenFromUserMessage()
        {
            _subject.Execute();

            _mockEventAggregator.Verify(x => x.Publish(It.IsAny<GetTokenFromUserMessage>()));
        }

        [Test]
        public void Execute_AfterHandlingTheTokenRequestResultAndTheRequestWasSuccessful_ReturnsAResultWithStatusSuccessfulAndTheToken()
        {
            var requestResult = new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Successful, "test");
            _mockEventAggregator.Setup(x => x.Publish(It.IsAny<GetTokenFromUserMessage>()))
                                .Callback(() => _subject.Handle(requestResult));

            var executeResult = _subject.Execute();

            executeResult.State.Should().Be(SimpleStepStateEnum.Successful);
            executeResult.Data.Should().Be("test");
        }

        [Test]
        public void Execute_AfterHandlingTheTokenRequestResultAndTheRequestWasCanceled_ReturnsAResultWithStatusSuccessfulAndTheToken()
        {
            var requestResult = new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Canceled);
            _mockEventAggregator.Setup(x => x.Publish(It.IsAny<GetTokenFromUserMessage>()))
                                .Callback(() => _subject.Handle(requestResult));

            var executeResult = _subject.Execute();

            executeResult.State.Should().Be(SimpleStepStateEnum.Failed);
            executeResult.Data.Should().BeNull();
        }
    }
}