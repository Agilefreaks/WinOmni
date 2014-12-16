namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Cryptography;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class RegisterDeviceTests
    {
        private RegisterDevice _subject;

        private Mock<IDevices> _mockDevices;

        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<ICryptoService> _mockCryptoService;

        [SetUp]
        public void Setup()
        {
            _mockDevices = new Mock<IDevices>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockCryptoService = new Mock<ICryptoService>();
            _subject = new RegisterDevice(_mockDevices.Object, _mockConfigurationService.Object, _mockCryptoService.Object);
        }

        [Test]
        public void Execute_Always_SavesKeyPair()
        {
            _mockConfigurationService.Setup(x => x.MachineName).Returns("testMachine");
            var testScheduler = new TestScheduler();
            SchedulerProvider.Default = testScheduler;
            var createDeviceObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<Device>>(100, Notification.CreateOnNext(new Device())),
                new Recorded<Notification<Device>>(200, Notification.CreateOnCompleted<Device>()));
            _mockDevices.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(createDeviceObservable);
            var keyPair = new KeyPair { Public = "publicKey" };
            _mockCryptoService.Setup(x => x.GenerateKeyPair()).Returns(keyPair);
            
            testScheduler.Start(_subject.Execute);

            _mockConfigurationService.VerifySet(x => x.DeviceKeyPair = keyPair);
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
            _mockDevices.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(createDeviceObservable);
            _mockCryptoService.Setup(x => x.GenerateKeyPair()).Returns(new KeyPair { Public = "publicKey" });
            
            testScheduler.Start(_subject.Execute);

            Guid newId;
            _mockDevices.Verify(
                x => x.Create(It.Is<string>(id => Guid.TryParse(id, out newId)), "testMachine", "publicKey"),
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
            _mockDevices.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(createDeviceObservable);
            _mockCryptoService.Setup(x => x.GenerateKeyPair()).Returns(new KeyPair { Public = "publicKey" });

            testScheduler.Start(_subject.Execute);

            _mockConfigurationService.VerifySet(x => x.DeviceIdentifier = NewDeviceId);
        }

        [Test]
        public void Execute_CreateDeviceIsSuccessful_CompletesWithASuccessfulResult()
        {
            var testScheduler = new TestScheduler();
            SchedulerProvider.Default = testScheduler;
            var createDeviceObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<Device>>(100, Notification.CreateOnNext(new Device())),
                new Recorded<Notification<Device>>(200, Notification.CreateOnCompleted<Device>()));
            _mockDevices.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(createDeviceObservable);
            _mockCryptoService.Setup(x => x.GenerateKeyPair()).Returns(new KeyPair { Public = "publicKey" });

            var testableObserver = testScheduler.Start(_subject.Execute);
                
            testableObserver.Messages.Count.Should().Be(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Successful);
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }
    }
}