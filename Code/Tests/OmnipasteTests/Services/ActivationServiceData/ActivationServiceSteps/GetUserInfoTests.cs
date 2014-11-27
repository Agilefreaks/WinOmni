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

        [SetUp]
        public void Setup()
        {
            _mockUserInfo = new Mock<IUserInfo>();
            _subject = new GetUserInfo(_mockUserInfo.Object);
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
        public void Execute_WhenSubscribedToAndGetUserInfoFails_WillCompletesWithFailedResult()
        {
            var exception = new Exception("test");
            var getUserInfoObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<UserInfo>>(100, Notification.CreateOnError<UserInfo>(exception)),
                    new Recorded<Notification<UserInfo>>(200, Notification.CreateOnCompleted<UserInfo>()));
            _mockUserInfo.Setup(x => x.Get()).Returns(() => getUserInfoObservable);

            var testableObserver = _testScheduler.Start(_subject.Execute, TimeSpan.FromSeconds(1).Ticks);

            testableObserver.Messages.Count.Should().Be(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Failed);
            testableObserver.Messages[0].Value.Value.Data.Should().Be(exception);
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }
    }
}