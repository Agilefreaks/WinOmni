namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Dto;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using Omnipaste.Framework.Services.ActivationServiceData;
    using Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class GetRemoteConfigurationTests
    {
        private GetRemoteConfiguration _subject;

        private Mock<IOAuth2> _mockOAuth2;

        private ITestableObserver<IExecuteResult> _observer;

        private readonly TestScheduler _testScheduler = new TestScheduler();

        [SetUp]
        public void Setup()
        {
            SchedulerProvider.Default = _testScheduler;
            _observer = _testScheduler.CreateObserver<IExecuteResult>();
            _mockOAuth2 = new Mock<IOAuth2>();

            _subject = new GetRemoteConfiguration(_mockOAuth2.Object);
        }

        [Test]
        public void Execute_WhenAuthorizationCodeIsEmpty_WillCompleteWithAFailedResult()
        {
            _subject.Parameter = new DependencyParameter(string.Empty, string.Empty);

            _subject.Execute().Subscribe(_observer);

            _observer.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            _observer.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Failed);
            _observer.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }

        [Test]
        public void Execute_WhenCreateError_WillCompleteWithAFailedResult()
        {
            _subject.Parameter = new DependencyParameter(string.Empty, "42");
            var createObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<TokenDto>>(0, Notification.CreateOnError<TokenDto>(new Exception())));
            _mockOAuth2.Setup(m => m.Create("42")).Returns(createObservable);

            _subject.Execute().Subscribe(_observer);
            _testScheduler.Start();

            _observer.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            _observer.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Failed);
            _observer.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }

        [Test]
        public void Execute_WhenCreateSuccess_WillCompleteWithASuccessResult()
        {
            _subject.Parameter = new DependencyParameter(string.Empty, "42");
            var testScheduler = _testScheduler;
            var createObservable =
                testScheduler.CreateColdObservable(
                    new Recorded<Notification<TokenDto>>(0, Notification.CreateOnNext(new TokenDto("acccess token", "refresh token"))),
                    new Recorded<Notification<TokenDto>>(0, Notification.CreateOnCompleted<TokenDto>()));
            _mockOAuth2.Setup(m => m.Create("42")).Returns(createObservable);

            _subject.Execute().Subscribe(_observer);
            testScheduler.Start(() => createObservable, 0, 0, TimeSpan.FromSeconds(1).Ticks);

            _observer.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            _observer.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Successful);
            _observer.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }
    }
}