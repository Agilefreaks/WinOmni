namespace Omnipaste.OmniClipboard.Infrastructure.Tests.Services.ActivationServiceData.ActivationServiceSteps
{
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class GetTokenFromUserTests
    {
        private GetTokenFromUser _subject;

        private Mock<IEventAggregator> _mockEventAggregator;

        private TokenRequestResultMessage _tokenRequestResultMessage;

        [SetUp]
        public void Setup()
        {
            this._mockEventAggregator = new Mock<IEventAggregator>();
            this._subject = new GetTokenFromUser(this._mockEventAggregator.Object);
            this._tokenRequestResultMessage =
                new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Successful, "test");

            this._mockEventAggregator.Setup(x => x.Publish(It.IsAny<GetTokenFromUserMessage>()))
                                .Callback(() => this._subject.Handle(this._tokenRequestResultMessage));
        }

        [Test]
        public void Ctor_Always_ShouldCallEventAggregatorSubscribeWithSelf()
        {
            this._mockEventAggregator.Verify(x => x.Subscribe(this._subject));
        }

        [Test]
        public void Execute_Always_ShouldCallEventAggregatorPublishWithTheGetTokenFromUserMessage()
        {
            this._subject.Execute();

            this._mockEventAggregator.Verify(x => x.Publish(It.IsAny<GetTokenFromUserMessage>()));
        }

        [Test]
        public void Execute_AfterHandlingTheTokenRequestResultAndTheRequestWasSuccessful_ReturnsAResultWithStatusSuccessfulAndTheToken()
        {
            var requestResult = new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Successful, "test");
            this._mockEventAggregator.Setup(x => x.Publish(It.IsAny<GetTokenFromUserMessage>()))
                                .Callback(() => this._subject.Handle(requestResult));

            var executeResult = this._subject.Execute();

            executeResult.State.Should().Be(SimpleStepStateEnum.Successful);
            executeResult.Data.Should().Be("test");
        }

        [Test]
        public void Execute_AfterHandlingTheTokenRequestResultAndTheRequestWasCanceled_ReturnsAResultWithStatusSuccessfulAndTheToken()
        {
            var requestResult = new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Canceled);
            this._mockEventAggregator.Setup(x => x.Publish(It.IsAny<GetTokenFromUserMessage>()))
                                .Callback(() => this._subject.Handle(requestResult));

            var executeResult = this._subject.Execute();

            executeResult.State.Should().Be(SimpleStepStateEnum.Failed);
            executeResult.Data.Should().BeNull();
        }
    }
}