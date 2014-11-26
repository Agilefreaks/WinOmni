namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class WaitForSecondDeviceTests
    {
        const int ObservableYieldTime = 100;
        const int ObservableCompletionTime = 200;

        private WaitForSecondDevice _subject;

        private Mock<IDevices> _mockDevices;

        private TimeSpan _checkInterval;

        private TestScheduler _testScheduler;

        [SetUp]
        public void Setup()
        {
            _mockDevices = new Mock<IDevices>();
            _checkInterval = TimeSpan.FromTicks(200);
            _subject = new WaitForSecondDevice(_mockDevices.Object, _checkInterval);
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
        }

        [Test]
        public void Execute_WhenSubscribedTo_WillReturnAValueAfterACallToGetDevicesReturnsAtLeast2Devices()
        {
            var devicesObservable1 =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<List<Device>>>(ObservableYieldTime, Notification.CreateOnNext(new List<Device>())),
                    new Recorded<Notification<List<Device>>>(ObservableCompletionTime, Notification.CreateOnCompleted<List<Device>>()));
            var devicesObservable2 =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<List<Device>>>(ObservableYieldTime, Notification.CreateOnNext(new List<Device> { new Device() })),
                    new Recorded<Notification<List<Device>>>(ObservableCompletionTime, Notification.CreateOnCompleted<List<Device>>()));
            var devices = new List<Device> { new Device(), new Device() };
            var devicesObservable3 =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<List<Device>>>(ObservableYieldTime, Notification.CreateOnNext(devices)),
                    new Recorded<Notification<List<Device>>>(ObservableCompletionTime, Notification.CreateOnCompleted<List<Device>>()));
            var deviceObservables = new[] { devicesObservable1, devicesObservable2, devicesObservable3 };
            var callCount = 0;
            _mockDevices.Setup(x => x.GetAll()).Returns(() => deviceObservables[callCount++]);

            var testableObserver = _testScheduler.Start(
                _subject.Execute,
                (ObservableCompletionTime + _checkInterval.Ticks) * deviceObservables.Length);

            testableObserver.Messages.Count.Should().Be(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Successful);
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
            _mockDevices.Verify(x => x.GetAll(), Times.Exactly(deviceObservables.Length));
        }

        [Test]
        public void Execute_WhenSubscribedTo_WillIgnoreErrorsUntilAtLeast2DevicesAreObtained()
        {
            var devicesObservable1 =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<List<Device>>>(ObservableYieldTime,
                        Notification.CreateOnError<List<Device>>(new Exception())),
                    new Recorded<Notification<List<Device>>>(ObservableCompletionTime, Notification.CreateOnCompleted<List<Device>>()));
            var devicesObservable2 =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<List<Device>>>(ObservableYieldTime,
                        Notification.CreateOnError<List<Device>>(new Exception())),
                    new Recorded<Notification<List<Device>>>(ObservableCompletionTime, Notification.CreateOnCompleted<List<Device>>()));
            var devices = new List<Device> { new Device(), new Device() };
            var devicesObservable3 =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<List<Device>>>(ObservableYieldTime, Notification.CreateOnNext(devices)),
                    new Recorded<Notification<List<Device>>>(ObservableCompletionTime, Notification.CreateOnCompleted<List<Device>>()));
            var deviceObservables = new[] { devicesObservable1, devicesObservable2, devicesObservable3 };
            var callCount = 0;
            _mockDevices.Setup(x => x.GetAll()).Returns(() => deviceObservables[callCount++]);

            var testableObserver = _testScheduler.Start(
                _subject.Execute,
                (ObservableCompletionTime + _checkInterval.Ticks) * deviceObservables.Length);

            testableObserver.Messages.Count.Should().Be(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Successful);
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
            _mockDevices.Verify(x => x.GetAll(), Times.Exactly(deviceObservables.Length));
        }

        [Test]
        public void Execute_WhenSubscribedTo_WillReportErrorsEncounteredWhileGettingDevices()
        {
            var exception = new Exception();
            var devicesObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<List<Device>>>(ObservableYieldTime,
                    Notification.CreateOnError<List<Device>>(exception)),
                new Recorded<Notification<List<Device>>>(ObservableCompletionTime, Notification.CreateOnCompleted<List<Device>>()));
            _mockDevices.Setup(x => x.GetAll()).Returns(devicesObservable);
            var mockExceptionReporter = new Mock<IExceptionReporter>();
            ExceptionReporter.Instance = mockExceptionReporter.Object;

            _testScheduler.Start(_subject.Execute, 2 * ObservableCompletionTime);

            mockExceptionReporter.Verify(x => x.Report(exception), Times.Once());
        }

        [Test]
        public void Execute_WhenSubscribedTo_WillNotReturnAValueIfAllCallsToGetDevicesReturnLessThan2Devices()
        {
            var testScheduler = new TestScheduler();
            var devicesObservable =
                testScheduler.CreateColdObservable(
                    new Recorded<Notification<List<Device>>>(ObservableYieldTime, Notification.CreateOnNext(new List<Device>())),
                    new Recorded<Notification<List<Device>>>(ObservableCompletionTime, Notification.CreateOnCompleted<List<Device>>()));
            _mockDevices.Setup(x => x.GetAll()).Returns(devicesObservable);

            var testableObserver = testScheduler.Start(_subject.Execute, 5 * _checkInterval.Ticks);

            testableObserver.Messages.Count.Should().Be(0);
        }
    }
}