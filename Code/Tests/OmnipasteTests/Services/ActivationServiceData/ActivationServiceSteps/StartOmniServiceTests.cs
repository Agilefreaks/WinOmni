namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using Omni;
    using OmniApi.Models;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

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
        public void Execute_WhenStartFails_WillReturnFailed()
        {
            TestScheduler testScheduler = new TestScheduler();
            var testObserver = testScheduler.CreateObserver<IExecuteResult>();
            var startObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<Device>>(0, Notification.CreateOnError<Device>(new Exception())));
            _omniService.Setup(m => m.Start()).Returns(startObservable);

            _subject.Execute().Subscribe(testObserver);
            testScheduler.Start();

            testObserver.Messages.Should()
                .Contain(
                    m => m.Value.Kind == NotificationKind.OnNext && m.Value.Value.State == SimpleStepStateEnum.Failed);
        }

        [Test]
        public void Execute_WhenStartSuccess_WillReturnSuccess()
        {
            TestScheduler testScheduler = new TestScheduler();
            var testObserver = testScheduler.CreateObserver<IExecuteResult>();
            var startObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<Device>>(0, Notification.CreateOnNext(new Device())),
                new Recorded<Notification<Device>>(0, Notification.CreateOnCompleted<Device>()));
            _omniService.Setup(m => m.Start()).Returns(startObservable);

            _subject.Execute().Subscribe(testObserver);
            testScheduler.Start();

            testObserver.Messages.Should()
                .Contain(
                    m => m.Value.Kind == NotificationKind.OnNext && m.Value.Value.State == SimpleStepStateEnum.Successful);
            testObserver.Messages.Should().HaveCount(2);
        }

    }
}