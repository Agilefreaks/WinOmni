namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class GetRemoteConfigurationTests
    {
        private GetRemoteConfiguration _subject;

        private Mock<IOAuth2> _mockOAuth2;

        private ITestableObserver<IExecuteResult> _observer;

        [SetUp]
        public void Setup()
        {
            _observer = new TestScheduler().CreateObserver<IExecuteResult>();
            _mockOAuth2 = new Mock<IOAuth2>();

            _subject = new GetRemoteConfiguration(_mockOAuth2.Object);
        }

        [Test]
        public void Execute_WhenAuthorizationCodeIsEmpty_WillReturnFail()
        {
            _subject.Parameter = new DependencyParameter(string.Empty, string.Empty);

            _subject.Execute().Subscribe(_observer);

            _observer.Messages.Should()
                .Contain(
                    m => m.Value.Kind == NotificationKind.OnNext
                        && m.Value.Value.State == SimpleStepStateEnum.Failed);
        }

        [Test]
        public void Execute_WhenCreateError_WillReturnFail()
        {
            _subject.Parameter = new DependencyParameter(string.Empty, "42");
            var testScheduler = new TestScheduler();
            var createObservable =
                testScheduler.CreateColdObservable(
                    new Recorded<Notification<Token>>(0, Notification.CreateOnError<Token>(new Exception())));
            _mockOAuth2.Setup(m => m.Create("42")).Returns(createObservable);

            _subject.Execute().Subscribe(_observer);
            testScheduler.Start(() => createObservable, 0, 0, TimeSpan.FromSeconds(1).Ticks);

             _observer.Messages.Should()
                .Contain(
                    m => m.Value.Kind == NotificationKind.OnNext
                        && m.Value.Value.State == SimpleStepStateEnum.Failed);
        }

        [Test]
        public void Execute_WhenCreateSuccess_WillReturnSucccess()
        {
            _subject.Parameter = new DependencyParameter(string.Empty, "42");
            var testScheduler = new TestScheduler();
            var createObservable =
                testScheduler.CreateColdObservable(
                    new Recorded<Notification<Token>>(0, Notification.CreateOnNext(new Token("acccess token", "refresh token"))),
                    new Recorded<Notification<Token>>(0, Notification.CreateOnCompleted<Token>()));
            _mockOAuth2.Setup(m => m.Create("42")).Returns(createObservable);

            _subject.Execute().Subscribe(_observer);
            testScheduler.Start(() => createObservable, 0, 0, TimeSpan.FromSeconds(1).Ticks);

            _observer.Messages.Should()
               .Contain(
                   m => m.Value.Kind == NotificationKind.OnNext
                       && m.Value.Value.State == SimpleStepStateEnum.Successful);
            _observer.Messages.Should().HaveCount(2);
        }
    }
}