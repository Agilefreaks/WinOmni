namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
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
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class EnsureEncryptionKeysTests
    {
        private const string Deviceid = "SomeDeviceId";

        private EnsureEncryptionKeys _subject;
        private Mock<ICryptoService> _mockCryptoService;
        private Mock<IConfigurationService> _mockConfigurationService;
        private Mock<IDevices> _mockDevices;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _mockCryptoService = new Mock<ICryptoService>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockDevices = new Mock<IDevices>();
            _mockConfigurationService.Setup(x => x.DeviceId).Returns(Deviceid);
            _subject = new EnsureEncryptionKeys(_mockCryptoService.Object, _mockConfigurationService.Object, _mockDevices.Object);
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void Execute_WhenConfigurationServiceDoesNotContainDeviceKeyPairAndUpdateSucceeds_SavesNewKeyPair()
        {
            var keyPair = new KeyPair { Public = "test", Private = "test" };
            _mockCryptoService.Setup(m => m.GenerateKeyPair()).Returns(keyPair);
            var updateObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<EmptyDto>>(100, Notification.CreateOnNext(new EmptyDto())),
                new Recorded<Notification<EmptyDto>>(200, Notification.CreateOnCompleted<EmptyDto>()));
            _mockDevices.Setup(m => m.Update(Deviceid, It.IsAny<object>())).Returns(updateObservable);

            _testScheduler.Start(() => _subject.Execute(), TimeSpan.FromSeconds(1).Ticks);

            _mockConfigurationService.VerifySet(x => x.DeviceKeyPair = keyPair);
        }

        [Test]
        public void Execute_WhenConfigurationServiceDoesNotContainDeviceKeyPairAndUpdateFails_DoesNotSaveNewKeyPair()
        {
            var keyPair = new KeyPair { Public = "test", Private = "test" };
            _mockCryptoService.Setup(m => m.GenerateKeyPair()).Returns(keyPair);
            var updateObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<EmptyDto>>(100, Notification.CreateOnError<EmptyDto>(new Exception())),
                new Recorded<Notification<EmptyDto>>(200, Notification.CreateOnCompleted<EmptyDto>()));
            KeyPair newValue = null;
            _mockConfigurationService.SetupSet(m => m.DeviceKeyPair = It.IsAny<KeyPair>()).Callback<KeyPair>(pair => newValue = pair);
            _mockDevices.Setup(m => m.Update(Deviceid, It.IsAny<object>())).Returns(updateObservable);

            _testScheduler.Start(() => _subject.Execute(), TimeSpan.FromSeconds(1).Ticks);

            newValue.Should().BeNull();
        }

        [Test]
        public void Execute_WhenConfigurationServiceContainsDeviceKeyPairWithEmptyPublicKey_SavesNewKeyPair()
        {
            var keyPair = new KeyPair { Public = "test", Private = "test" };
            _mockCryptoService.Setup(m => m.GenerateKeyPair()).Returns(keyPair);
            var updateObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<EmptyDto>>(100, Notification.CreateOnNext(new EmptyDto())),
                new Recorded<Notification<EmptyDto>>(200, Notification.CreateOnCompleted<EmptyDto>()));
            _mockConfigurationService.SetupGet(x => x.DeviceKeyPair).Returns(new KeyPair { Private = "test" });
            _mockDevices.Setup(m => m.Update(Deviceid, It.IsAny<object>())).Returns(updateObservable);

            _testScheduler.Start(() => _subject.Execute(), TimeSpan.FromSeconds(1).Ticks);

            _mockConfigurationService.VerifySet(x => x.DeviceKeyPair = keyPair);
        }
        
        [Test]
        public void Execute_WhenConfigurationServiceContainsDeviceKeyPairWithValues_CreatesAKeyPair()
        {
            _mockConfigurationService.SetupGet(m => m.DeviceKeyPair).Returns(new KeyPair { Public = "test", Private = "test" });
            _subject.Execute().Wait();

            _mockCryptoService.Verify(m => m.GenerateKeyPair(), Times.Never());
        }
    }
}
