namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Cryptography;
    using OmniApi.Dto;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class RegisterDeviceTests
    {
        private RegisterDevice _subject;

        private Mock<IDevices> _mockDevices;

        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<ICryptoService> _mockCryptoService;

        private TestScheduler _testScheduler;

        [SetUp]
        public void Setup()
        {
            _mockDevices = new Mock<IDevices> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockCryptoService = new Mock<ICryptoService>();
            _subject = new RegisterDevice(
                _mockDevices.Object,
                _mockConfigurationService.Object,
                _mockCryptoService.Object);
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
                new Recorded<Notification<DeviceDto>>(100, Notification.CreateOnNext(new DeviceDto())),
                new Recorded<Notification<DeviceDto>>(200, Notification.CreateOnCompleted<DeviceDto>()));
            _mockDevices.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>())).Returns(createDeviceObservable);
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
                new Recorded<Notification<DeviceDto>>(100, Notification.CreateOnNext(new DeviceDto())),
                new Recorded<Notification<DeviceDto>>(200, Notification.CreateOnCompleted<DeviceDto>()));
            _mockDevices.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>())).Returns(createDeviceObservable);
            _mockCryptoService.Setup(x => x.GenerateKeyPair()).Returns(new KeyPair { Public = "publicKey" });
            
            _testScheduler.Start(_subject.Execute);

            _mockDevices.Verify(x => x.Create("testMachine", "publicKey"), Times.Once());
        }

        [Test]
        public void Execute_CreateDeviceIsSuccessful_SavesTheNewDeviceId()
        {
            const string NewDeviceId = "someNewId";
            var createDeviceObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<DeviceDto>>(100, Notification.CreateOnNext(new DeviceDto { Id = NewDeviceId  })),
                new Recorded<Notification<DeviceDto>>(200, Notification.CreateOnCompleted<DeviceDto>()));
            _mockDevices.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>())).Returns(createDeviceObservable);
            _mockCryptoService.Setup(x => x.GenerateKeyPair()).Returns(new KeyPair { Public = "publicKey" });

            _testScheduler.Start(_subject.Execute);

            _mockConfigurationService.VerifySet(x => x.DeviceId = NewDeviceId);
        }

        [Test]
        public void Execute_CreateDeviceIsSuccessful_CompletesWithASuccessfulResult()
        {
            var createDeviceObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<DeviceDto>>(100, Notification.CreateOnNext(new DeviceDto())),
                new Recorded<Notification<DeviceDto>>(200, Notification.CreateOnCompleted<DeviceDto>()));
            _mockDevices.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>())).Returns(createDeviceObservable);
            _mockCryptoService.Setup(x => x.GenerateKeyPair()).Returns(new KeyPair { Public = "publicKey" });

            var testableObserver = _testScheduler.Start(_subject.Execute);
                
            testableObserver.Messages.Count.Should().Be(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Successful);
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }
    }
}