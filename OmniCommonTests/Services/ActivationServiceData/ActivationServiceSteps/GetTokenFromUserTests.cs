namespace OmniCommonTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
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

        private TokenRequestResutMessage _tokenRequestResutMessage;

        [SetUp]
        public void Setup()
        {
            _mockEventAggregator = new Mock<IEventAggregator>();
            _subject = new GetTokenFromUser(_mockEventAggregator.Object);
            _tokenRequestResutMessage = new TokenRequestResutMessage
                                            {
                                                Status = TokenRequestResultMessageStatusEnum.Successful,
                                                Token = "test"
                                            };
            _mockEventAggregator.Setup(x => x.Publish(It.IsAny<GetTokenFromUserMessage>()))
                                .Callback(() => _subject.Handle(_tokenRequestResutMessage));
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
        public void Execute_Always_ShouldWaitToReceiveAHandleTokenRequestResult()
        {
            var waited = false;
            _subject.OnTokenRequestResultAction = message => { waited = true; };
            _mockEventAggregator.Setup(x => x.Publish(It.IsAny<GetTokenFromUserMessage>()))
                                .Callback(
                                    () => Task.Factory.StartNew(
                                        () =>
                                        {
                                            Thread.Sleep(500);
                                            _subject.Handle(new TokenRequestResutMessage());
                                        }));

            _subject.Execute();

            waited.Should().BeTrue();
        }

        [Test]
        public void Ctor_Always_ShouldSetOnTokenRequestResultActionToOnTokenRequestResult()
        {
            _subject.OnTokenRequestResultAction.Should()
                    .Be((Action<TokenRequestResutMessage>)_subject.OnTokenRequestResult);
        }

        [Test]
        public void Execute_AfterHandlingTheTokenRequestResultAndTheRequestWasSuccessful_ReturnsAResultWithStatusSuccessfulAndTheToken()
        {
            var requestResult = new TokenRequestResutMessage
                                    {
                                        Status = TokenRequestResultMessageStatusEnum.Successful,
                                        Token = "test"
                                    };
            _mockEventAggregator.Setup(x => x.Publish(It.IsAny<GetTokenFromUserMessage>()))
                                .Callback(() => _subject.Handle(requestResult));

            var executeResult = _subject.Execute();

            executeResult.State.Should().Be(SimpleStepStateEnum.Successful);
            executeResult.Data.Should().Be("test");
        }

        [Test]
        public void Execute_AfterHandlingTheTokenRequestResultAndTheRequestWasCanceled_ReturnsAResultWithStatusSuccessfulAndTheToken()
        {
            var requestResult = new TokenRequestResutMessage
                                    {
                                        Status = TokenRequestResultMessageStatusEnum.Canceled
                                    };
            _mockEventAggregator.Setup(x => x.Publish(It.IsAny<GetTokenFromUserMessage>()))
                                .Callback(() => _subject.Handle(requestResult));

            var executeResult = _subject.Execute();

            executeResult.State.Should().Be(SimpleStepStateEnum.Failed);
            executeResult.Data.Should().BeNull();
        }
    }
}