namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Collections.Generic;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using Omni;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Models;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class WaitForSecondDeviceTests
    {
        private WaitForSecondDevice _subject;

        private Mock<IOmniService> _mockOmniService;

        private Mock<IDevices> _mockDevices;

        [SetUp]
        public void Setup()
        {
            _mockOmniService = new Mock<IOmniService>();
            _mockDevices = new Mock<IDevices>();
            _subject = new WaitForSecondDevice(_mockOmniService.Object, _mockDevices.Object);
        }

        [Test]
        public void Execute_WhenSubscribedTo_WillReturnAValueAfterADeviceMessageIsReceivedAndTheGetDevicesMethodReturns2Devices()
        {
            var testScheduler = new TestScheduler();
            var omniMessageObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<OmniMessage>>(
                    100,
                    Notification.CreateOnNext(new OmniMessage(OmniMessageTypeEnum.Device))));
            _mockOmniService.SetupGet(x => x.OmniMessageObservable).Returns(omniMessageObservable);
            var devices = new List<Device> { new Device(), new Device() };
            var devicesObservable =
                testScheduler.CreateColdObservable(
                    new Recorded<Notification<List<Device>>>(200, Notification.CreateOnNext(devices)),
                    new Recorded<Notification<List<Device>>>(300, Notification.CreateOnCompleted<List<Device>>()));
            _mockDevices.Setup(x => x.GetAll()).Returns(devicesObservable);

            var testableObserver = testScheduler.Start(_subject.Execute);

            testableObserver.Messages.Count.Should().Be(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Successful);
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }

        [Test]
        public void Execute_WhenSubscribedTo_WillNotReturnAValueIfADeviceMessageIsReceivedButCallToGetDevicesMethodNeverReturnsMoreThan1Device()
        {
            var testScheduler = new TestScheduler();
            var omniMessageObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<OmniMessage>>(
                    100,
                    Notification.CreateOnNext(new OmniMessage(OmniMessageTypeEnum.Device))));
            _mockOmniService.SetupGet(x => x.OmniMessageObservable).Returns(omniMessageObservable);
            var devicesObservable =
                testScheduler.CreateColdObservable(
                    new Recorded<Notification<List<Device>>>(200, Notification.CreateOnNext(new List<Device>())),
                    new Recorded<Notification<List<Device>>>(300, Notification.CreateOnCompleted<List<Device>>()));
            _mockDevices.Setup(x => x.GetAll()).Returns(devicesObservable);

            var testableObserver = testScheduler.Start(_subject.Execute);

            testableObserver.Messages.Count.Should().Be(0);
        }

        [Test]
        public void Execute_WhenSubscribedTo_WillNotReturnAValueIfADeviceMessageIsNeverReceived()
        {
            var testScheduler = new TestScheduler();
            var omniMessageObservable =
                testScheduler.CreateColdObservable(
                    new Recorded<Notification<OmniMessage>>(100, Notification.CreateOnCompleted<OmniMessage>()));
            _mockOmniService.SetupGet(x => x.OmniMessageObservable).Returns(omniMessageObservable);

            var testableObserver = testScheduler.Start(_subject.Execute);

            testableObserver.Messages.Count.Should().Be(1);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }
    }
}