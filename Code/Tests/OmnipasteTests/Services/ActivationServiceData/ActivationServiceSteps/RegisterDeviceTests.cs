namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Cryptography;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using OmniCommon.Settings;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class RegisterDeviceTests
    {
        private RegisterDevice _subject;

        private Mock<IDevices> _mockDevices;

        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<ICryptoService> _mockCryptoService;

        private Mock<IConfigurationContainer> _mockConfigurationContainer;

        private TestScheduler _testScheduler;

        [SetUp]
        public void Setup()
        {
            _mockDevices = new Mock<IDevices> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockCryptoService = new Mock<ICryptoService>();
            _mockConfigurationContainer = new Mock<IConfigurationContainer>();
            _subject = new RegisterDevice(
                _mockDevices.Object,
                _mockConfigurationService.Object,
                _mockCryptoService.Object,
                _mockConfigurationContainer.Object);
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void Execute_Always_SavesKeyPair()
        {
            _mockConfigurationService.Setup(x => x.MachineName).Returns("testMachine");
            var createDeviceObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<Device>>(100, Notification.CreateOnNext(new Device())),
                new Recorded<Notification<Device>>(200, Notification.CreateOnCompleted<Device>()));
            _mockDevices.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(createDeviceObservable);
            var keyPair = new KeyPair { Public = "publicKey" };
            _mockCryptoService.Setup(x => x.GenerateKeyPair()).Returns(keyPair);
            
            _testScheduler.Start(_subject.Execute);

            _mockConfigurationService.VerifySet(x => x.DeviceKeyPair = keyPair);
        }

        [Test]
        public void Execute_ADeviceIdentifierDoesNotExist_TriesToCreateANewDeviceWithTheMachineNameAndAGuid()
        {
            _mockConfigurationService.Setup(x => x.MachineName).Returns("testMachine");
            var createDeviceObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<Device>>(100, Notification.CreateOnNext(new Device())),
                new Recorded<Notification<Device>>(200, Notification.CreateOnCompleted<Device>()));
            _mockDevices.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(createDeviceObservable);
            _mockCryptoService.Setup(x => x.GenerateKeyPair()).Returns(new KeyPair { Public = "publicKey" });
            
            _testScheduler.Start(_subject.Execute);

            Guid newId;
            _mockDevices.Verify(
                x => x.Create(It.Is<string>(id => Guid.TryParse(id, out newId)), "testMachine", "publicKey"),
                Times.Once());
        }

        [Test]
        public void Execute_CreateDeviceIsSuccessful_SavesTheNewDeviceId()
        {
            const string NewDeviceId = "someNewId";
            var createDeviceObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<Device>>(100, Notification.CreateOnNext(new Device(NewDeviceId))),
                new Recorded<Notification<Device>>(200, Notification.CreateOnCompleted<Device>()));
            _mockDevices.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(createDeviceObservable);
            _mockCryptoService.Setup(x => x.GenerateKeyPair()).Returns(new KeyPair { Public = "publicKey" });

            _testScheduler.Start(_subject.Execute);

            _mockConfigurationService.VerifySet(x => x.DeviceIdentifier = NewDeviceId);
        }

        [Test]
        public void Execute_CreateDeviceIsSuccessful_CompletesWithASuccessfulResult()
        {
            var createDeviceObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<Device>>(100, Notification.CreateOnNext(new Device())),
                new Recorded<Notification<Device>>(200, Notification.CreateOnCompleted<Device>()));
            _mockDevices.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(createDeviceObservable);
            _mockCryptoService.Setup(x => x.GenerateKeyPair()).Returns(new KeyPair { Public = "publicKey" });

            var testableObserver = _testScheduler.Start(_subject.Execute);
                
            testableObserver.Messages.Count.Should().Be(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Successful);
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }

        [Test]
        public void Execute_ADeviceIdentifierExists_DoesNotRegisterANewDevice()
        {
            _mockConfigurationService.SetupGet(x => x.DeviceIdentifier).Returns("someIdentifier");

            _testScheduler.Start(_subject.Execute);

            _mockDevices.Verify(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),Times.Never());
        }

        [Test]
        public void Execute_ADeviceIdentifierExists_GetsAllTheUsersDevices()
        {
            _mockConfigurationService.SetupGet(x => x.DeviceIdentifier).Returns("someIdentifier");
            var devices = new List<Device>();
            var getDevicesObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<List<Device>>>(100, Notification.CreateOnNext(devices)),
                new Recorded<Notification<List<Device>>>(200, Notification.CreateOnCompleted<List<Device>>()));
            _mockDevices.Setup(x => x.GetAll()).Returns(getDevicesObservable);

            _testScheduler.Start(_subject.Execute);

            _mockDevices.Verify(x => x.GetAll(), Times.Once());
        }

        [Test]
        public void Execute_ADeviceIdentifierExistsAndGettingAllTheDevicesReturnsADeviceListContainingTheCurrentDevice_SetsTheDeviceIdInTheConfigurationContainer()
        {
            _mockConfigurationService.SetupGet(x => x.DeviceIdentifier).Returns("someIdentifier");
            var devices = new List<Device>
                              {
                                  new Device("identifier1"),
                                  new Device("test") { Name = "someIdentifier", Id = "someId" },
                                  new Device("identifier3")
                              };
            var getDevicesObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<List<Device>>>(100, Notification.CreateOnNext(devices)),
                new Recorded<Notification<List<Device>>>(200, Notification.CreateOnCompleted<List<Device>>()));
            _mockDevices.Setup(x => x.GetAll()).Returns(getDevicesObservable);

            _testScheduler.Start(_subject.Execute);

            _mockConfigurationContainer.Verify(x => x.SetValue(ConfigurationProperties.DeviceId, "someId"));
        }
    }
}