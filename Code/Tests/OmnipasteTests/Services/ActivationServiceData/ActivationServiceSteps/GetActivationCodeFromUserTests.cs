namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;
    using Action = System.Action;

    [TestFixture]
    public class GetActivationCodeFromUserTests
    {
        private Mock<IEventAggregator> _eventAggregator;

        private GetActivationCodeFromUser _subject;

        private ITestableObserver<IExecuteResult> _observer;

        [SetUp]
        public void SetUp()
        {
            _observer = new TestScheduler().CreateObserver<IExecuteResult>();
            _eventAggregator = new Mock<IEventAggregator>();
            _subject = new GetActivationCodeFromUser(_eventAggregator.Object)
                           {
                               Parameter = new DependencyParameter(string.Empty, string.Empty)
                           };
        }

        [Test]
        public void Execute_Always_PublishesEventInObserver()
        {
            _subject.Execute();
            _eventAggregator.Verify(m => m.Publish(It.IsAny<GetTokenFromUserMessage>(), It.IsAny<Action<Action>>()));
        }

        [Test]
        public void Execute_WhenCanceled_WillFail()
        {
            _subject.Execute().Subscribe(_observer);
            _subject.Handle(new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Canceled));

            _observer.Messages.Should()
                .Contain(m => m.Value.Kind == NotificationKind.OnNext
                    && m.Value.Value.State == SimpleStepStateEnum.Failed);
        }

        [Test]
        public void Execute_WhenSuccessful_WillSucced()
        {
            _subject.Execute().Subscribe(_observer);
            _subject.Handle(new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Successful));

            _observer.Messages.Should()
                .Contain(m => m.Value.Kind == NotificationKind.OnNext
                    && m.Value.Value.State == SimpleStepStateEnum.Successful);
        }
    }
}