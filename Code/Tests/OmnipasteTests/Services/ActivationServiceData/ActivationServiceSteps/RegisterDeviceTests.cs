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
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class RegisterDeviceTests
    {
        private RegisterDevice _subject;

        private Mock<IDevices> _mockDevices;

        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void Setup()
        {
            _mockDevices = new Mock<IDevices>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _subject = new RegisterDevice(_mockDevices.Object, _mockConfigurationService.Object);
        }

        [Test]
        public void Execute_Always_TriesToCreateANewDeviceWithTheMachineNameAndAGuid()
        {
            _mockConfigurationService.Setup(x => x.MachineName).Returns("testMachine");
            var testScheduler = new TestScheduler();
            SchedulerProvider.Default = testScheduler;
            var createDeviceObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<Device>>(100, Notification.CreateOnNext(new Device())),
                new Recorded<Notification<Device>>(200, Notification.CreateOnCompleted<Device>()));
            _mockDevices.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>())).Returns(createDeviceObservable);
            
            testScheduler.Start(_subject.Execute);

            Guid newId;
            _mockDevices.Verify(
                x => x.Create(It.Is<string>(id => Guid.TryParse(id, out newId)), "testMachine"),
                Times.Once());
        }

        [Test]
        public void Execute_CreateDeviceIsSuccessful_SavesTheNewDeviceId()
        {
            const string NewDeviceId = "someNewId";
            var testScheduler = new TestScheduler();
            SchedulerProvider.Default = testScheduler;
            var createDeviceObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<Device>>(100, Notification.CreateOnNext(new Device(NewDeviceId))),
                new Recorded<Notification<Device>>(200, Notification.CreateOnCompleted<Device>()));
            _mockDevices.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>())).Returns(createDeviceObservable);

            testScheduler.Start(_subject.Execute);

            _mockConfigurationService.VerifySet(x => x.DeviceIdentifier = NewDeviceId);
        }

        [Test]
        public void Execute_CreateDeviceIsSuccessful_CompletesWithASuccessfulResultWhichHasTheNewDevice()
        {
            var testScheduler = new TestScheduler();
            SchedulerProvider.Default = testScheduler;
            var newDevice = new Device();
            var createDeviceObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<Device>>(100, Notification.CreateOnNext(newDevice)),
                new Recorded<Notification<Device>>(200, Notification.CreateOnCompleted<Device>()));
            _mockDevices.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>())).Returns(createDeviceObservable);

            var testableObserver = testScheduler.Start(_subject.Execute);
                
            testableObserver.Messages.Count.Should().Be(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Successful);
            testableObserver.Messages[0].Value.Value.Data.Should().Be(newDevice);
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }
    }
}