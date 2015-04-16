namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Dto;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps;

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
                    new Recorded<Notification<List<DeviceDto>>>(ObservableYieldTime, Notification.CreateOnNext(new List<DeviceDto>())),
                    new Recorded<Notification<List<DeviceDto>>>(ObservableCompletionTime, Notification.CreateOnCompleted<List<DeviceDto>>()));
            var devicesObservable2 =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<List<DeviceDto>>>(ObservableYieldTime, Notification.CreateOnNext(new List<DeviceDto> { new DeviceDto() })),
                    new Recorded<Notification<List<DeviceDto>>>(ObservableCompletionTime, Notification.CreateOnCompleted<List<DeviceDto>>()));
            var devices = new List<DeviceDto> { new DeviceDto(), new DeviceDto() };
            var devicesObservable3 =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<List<DeviceDto>>>(ObservableYieldTime, Notification.CreateOnNext(devices)),
                    new Recorded<Notification<List<DeviceDto>>>(ObservableCompletionTime, Notification.CreateOnCompleted<List<DeviceDto>>()));
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
        public void Execute_WhenSubscribedTo_WillFailOnErrors()
        {
            var exception = new Exception();
            var devicesObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<List<DeviceDto>>>(ObservableYieldTime,
                        Notification.CreateOnError<List<DeviceDto>>(exception)),
                    new Recorded<Notification<List<DeviceDto>>>(ObservableCompletionTime, Notification.CreateOnCompleted<List<DeviceDto>>()));
            _mockDevices.Setup(x => x.GetAll()).Returns(devicesObservable);

            var testableObserver = _testScheduler.Start(_subject.Execute, TimeSpan.FromSeconds(1).Ticks);

            testableObserver.Messages.Count.Should().Be(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Failed);
            testableObserver.Messages[0].Value.Value.Data.Should().Be(exception);
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }

        [Test]
        public void Execute_WhenSubscribedTo_WillNotReturnAValueIfAllCallsToGetDevicesReturnLessThan2Devices()
        {
            var testScheduler = new TestScheduler();
            var devicesObservable =
                testScheduler.CreateColdObservable(
                    new Recorded<Notification<List<DeviceDto>>>(ObservableYieldTime, Notification.CreateOnNext(new List<DeviceDto>())),
                    new Recorded<Notification<List<DeviceDto>>>(ObservableCompletionTime, Notification.CreateOnCompleted<List<DeviceDto>>()));
            _mockDevices.Setup(x => x.GetAll()).Returns(devicesObservable);

            var testableObserver = testScheduler.Start(_subject.Execute, 5 * _checkInterval.Ticks);

            testableObserver.Messages.Count.Should().Be(0);
        }
    }
}