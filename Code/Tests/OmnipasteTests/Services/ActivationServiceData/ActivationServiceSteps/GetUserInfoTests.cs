namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;
    using UserInfo = OmniApi.Models.UserInfo;

    [TestFixture]
    public class GetUserInfoTests
    {
        private GetUserInfo _subject;

        private Mock<IUserInfo> _mockUserInfo;

        private TestScheduler _testScheduler;

        private UserInfo _userInfo;

        private TimeSpan _retryDelay;

        private int _retryCount;

        [SetUp]
        public void Setup()
        {
            _mockUserInfo = new Mock<IUserInfo>();
            _retryDelay = TimeSpan.FromTicks(100);
            _retryCount = 3;
            _subject = new GetUserInfo(_mockUserInfo.Object, _retryDelay, _retryCount);
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            _userInfo = new UserInfo();
            var getUserInfoObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<UserInfo>>(100, Notification.CreateOnNext(_userInfo)),
                new Recorded<Notification<UserInfo>>(200, Notification.CreateOnCompleted<UserInfo>()));
            _mockUserInfo.Setup(x => x.Get()).Returns(getUserInfoObservable);
        }

        [Test]
        public void Execute_WhenSubscribedTo_CallsGetUserInfo()
        {
            _testScheduler.Start(_subject.Execute);

            _mockUserInfo.Verify(x => x.Get(), Times.Once);
        }

        [Test]
        public void Execute_WhenSubscribedToAndGetUserInfoReturnsAResult_CompletesSuccessfullyWithTheObtainedUserInfo()
        {
            var testableObserver = _testScheduler.Start(_subject.Execute);

            testableObserver.Messages.Count.Should().Be(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Successful);
            testableObserver.Messages[0].Value.Value.Data.Should().Be(_userInfo);
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }

        [Test]
        public void Execute_WhenSubscribedToAndGetUserInfoFails_ReportErrors()
        {
            var exception = new Exception("test");
            var getUserInfoObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<UserInfo>>(100, Notification.CreateOnError<UserInfo>(exception)),
                    new Recorded<Notification<UserInfo>>(200, Notification.CreateOnCompleted<UserInfo>()));
            _mockUserInfo.Setup(x => x.Get()).Returns(getUserInfoObservable);
            var mockExceptionReporter = new Mock<IExceptionReporter>();
            ExceptionReporter.Instance = mockExceptionReporter.Object;

            _testScheduler.Start(_subject.Execute, TimeSpan.FromSeconds(1).Ticks);

            mockExceptionReporter.Verify(x => x.Report(exception));
        }


        [Test]
        public void Execute_WhenSubscribedToAndGetUserInfoFails_WillTryAgainAndIfAResultIsObtainedCompletesSuccessfully()
        {
            var getUserInfoObservable1 =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<UserInfo>>(100, Notification.CreateOnError<UserInfo>(new Exception("test"))),
                    new Recorded<Notification<UserInfo>>(200, Notification.CreateOnCompleted<UserInfo>()));
            var getUserInfoObservable2 =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<UserInfo>>(100, Notification.CreateOnError<UserInfo>(new Exception("test"))),
                    new Recorded<Notification<UserInfo>>(200, Notification.CreateOnCompleted<UserInfo>()));
            var getUserInfoObservable3 =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<UserInfo>>(100, Notification.CreateOnNext(_userInfo)),
                    new Recorded<Notification<UserInfo>>(200, Notification.CreateOnCompleted<UserInfo>()));
            var observables = new[] { getUserInfoObservable1, getUserInfoObservable2, getUserInfoObservable3 };
            var callCount = 0;
            _mockUserInfo.Setup(x => x.Get()).Returns(() => observables[callCount++]);

            var testableObserver = _testScheduler.Start(_subject.Execute, 3 * (200 + _retryDelay.Ticks));

            _mockUserInfo.Verify(x => x.Get(), Times.Exactly(3));
            testableObserver.Messages.Count.Should().Be(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Successful);
            testableObserver.Messages[0].Value.Value.Data.Should().Be(_userInfo);
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }

        [Test]
        public void Execute_WhenSubscribedToAndGetUserInfoFailsMoreThanTheAllowedNumberOfRetries_WillCompleteWithAFailedResult()
        {
            var getUserInfoObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<UserInfo>>(100, Notification.CreateOnError<UserInfo>(new Exception("test"))),
                    new Recorded<Notification<UserInfo>>(200, Notification.CreateOnCompleted<UserInfo>()));
            _mockUserInfo.Setup(x => x.Get()).Returns(() => getUserInfoObservable);

            var testableObserver = _testScheduler.Start(_subject.Execute, (_retryCount + 1) * (200 + _retryDelay.Ticks));

            testableObserver.Messages.Count.Should().Be(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Failed);
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }
    }
}