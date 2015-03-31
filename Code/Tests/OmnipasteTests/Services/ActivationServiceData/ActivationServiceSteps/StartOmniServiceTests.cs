namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using Omni;
    using Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class StartOmniServiceTests
    {
        private StartOmniService _subject;

        private Mock<IOmniService> _omniService;

        [SetUp]
        public void SetUp()
        {
            _omniService = new Mock<IOmniService>();
            _subject = new StartOmniService(_omniService.Object);
        }

        [Test]
        public void Execute_WhenStartFails_WillCompleteWithOnError()
        {
            var testScheduler = new TestScheduler();
            var testObserver = testScheduler.CreateObserver<IExecuteResult>();
            var startObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<Unit>>(0, Notification.CreateOnError<Unit>(new Exception())));
            _omniService.Setup(m => m.Start()).Returns(startObservable);

            _subject.Execute().Subscribe(testObserver);
            testScheduler.Start();

            testObserver.Messages.Should()
                .Contain(
                    m => m.Value.Kind == NotificationKind.OnError);
        }

        [Test]
        public void Execute_WhenStartSuccess_WillReturnSuccess()
        {
            var testScheduler = new TestScheduler();
            var testObserver = testScheduler.CreateObserver<IExecuteResult>();
            var startObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<Unit>>(0, Notification.CreateOnNext(new Unit())),
                new Recorded<Notification<Unit>>(0, Notification.CreateOnCompleted<Unit>()));
            _omniService.Setup(m => m.Start()).Returns(startObservable);

            _subject.Execute().Subscribe(testObserver);
            testScheduler.Start();

            testObserver.Messages.Should()
                .Contain(
                    m => m.Value.Kind == NotificationKind.OnNext && m.Value.Value.State.Equals(SimpleStepStateEnum.Successful));
            testObserver.Messages.Should().HaveCount(2);
        }

        [Test]
        public void Execute_WhenStartCompletesWithoutAResult_WillReturnFail()
        {
            var testScheduler = new TestScheduler();
            var testObserver = testScheduler.CreateObserver<IExecuteResult>();
            var startObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<Unit>>(0, Notification.CreateOnCompleted<Unit>()));
            _omniService.Setup(m => m.Start()).Returns(startObservable);

            _subject.Execute().Subscribe(testObserver);
            testScheduler.Start();

            testObserver.Messages.Should().HaveCount(2);
            testObserver.Messages.Should().Contain(m => m.Value.Kind == NotificationKind.OnNext &&
                m.Value.Value.State.Equals(SimpleStepStateEnum.Failed));
            testObserver.Messages.Should().Contain(m => m.Value.Kind == NotificationKind.OnCompleted);
        }

    }
}