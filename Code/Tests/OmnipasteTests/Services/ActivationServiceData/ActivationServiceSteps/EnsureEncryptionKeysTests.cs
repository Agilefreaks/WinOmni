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
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class EnsureEncryptionKeysTests
    {
        private EnsureEncryptionKeys _subject;
        private Mock<ICryptoService> _mockCryptoService;
        private Mock<IConfigurationService> _mockConfigurationService;
        private Mock<IDevices> _mockDevices;

        [SetUp]
        public void SetUp()
        {
            _mockCryptoService = new Mock<ICryptoService>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockDevices = new Mock<IDevices>();
            _subject = new EnsureEncryptionKeys(_mockCryptoService.Object, _mockConfigurationService.Object, _mockDevices.Object);
        }

        [Test]
        public void Execute_WhenConfigurationServiceDoesNotContainDeviceKeyPairAndUpdateSucceeds_SavesNewKeyPair()
        {
            var keyPair = new KeyPair { Public = "test", Private = "test" };
            _mockCryptoService.Setup(m => m.GenerateKeyPair()).Returns(keyPair);
            var testScheduler = new TestScheduler();
            var updateObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<EmptyModel>>(100, Notification.CreateOnNext(new EmptyModel())),
                new Recorded<Notification<EmptyModel>>(200, Notification.CreateOnCompleted<EmptyModel>()));

            _mockDevices.Setup(m => m.Update(It.IsAny<object>())).Returns(updateObservable);

            testScheduler.Start(() => _subject.Execute(), TimeSpan.FromSeconds(1).Ticks);

            _mockConfigurationService.VerifySet(x => x.DeviceKeyPair = keyPair);
        }

        [Test]
        public void Execute_WhenConfigurationServiceDoesNotContainDeviceKeyPairAndUpdateFails_DoesNotSaveNewKeyPair()
        {
            var keyPair = new KeyPair { Public = "test", Private = "test" };
            _mockCryptoService.Setup(m => m.GenerateKeyPair()).Returns(keyPair);
            var testScheduler = new TestScheduler();
            var updateObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<EmptyModel>>(100, Notification.CreateOnError<EmptyModel>(new Exception())),
                new Recorded<Notification<EmptyModel>>(200, Notification.CreateOnCompleted<EmptyModel>()));
            KeyPair newValue = null;
            _mockConfigurationService.SetupSet(m => m.DeviceKeyPair).Callback(pair => newValue = pair);

            _mockDevices.Setup(m => m.Update(It.IsAny<object>())).Returns(updateObservable);

            testScheduler.Start(() => _subject.Execute());

            newValue.Should().BeNull();
        }

        [Test]
        public void Execute_WhenConfigurationServiceContainsDeviceKeyPairWithEmptyPublicKey_SavesNewKeyPair()
        {
            var keyPair = new KeyPair { Public = "test", Private = "test" };
            _mockCryptoService.Setup(m => m.GenerateKeyPair()).Returns(keyPair);
            var testScheduler = new TestScheduler();
            var updateObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<EmptyModel>>(100, Notification.CreateOnNext(new EmptyModel())),
                new Recorded<Notification<EmptyModel>>(200, Notification.CreateOnCompleted<EmptyModel>()));
            _mockConfigurationService.SetupGet(x => x.DeviceKeyPair).Returns(new KeyPair { Private = "test" });

            _mockDevices.Setup(m => m.Update(It.IsAny<object>())).Returns(updateObservable);

            testScheduler.Start(() => _subject.Execute(), TimeSpan.FromSeconds(1).Ticks);

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
