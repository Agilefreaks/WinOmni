namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Reactive.Linq;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Cryptography;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class CreateEncryptionKeysTests
    {
        private CreateEncryptionKeys _subject;
        private Mock<ICryptoService> _mockCryptoService;
        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void SetUp()
        {
            _mockCryptoService = new Mock<ICryptoService>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _subject = new CreateEncryptionKeys(_mockCryptoService.Object, _mockConfigurationService.Object);
        }

        [Test]
        public void Execute_WhenConfigurationServiceDoesNotContainDeviceKeyPair_CreatesAKeyPair()
        {
            var keyPair = new KeyPair { Public = "test", Private = "test" };
            _mockCryptoService.Setup(m => m.GenerateKeyPair()).Returns(keyPair);

            _subject.Execute().Wait();

            _mockConfigurationService.VerifySet(x => x.DeviceKeyPair = keyPair);
        }

        [Test]
        public void Execute_WhenConfigurationServiceContainsDeviceKeyPairWithEmptyPublicKey_CreatesAKeyPair()
        {
            var newKeyPair = new KeyPair { Public = "test", Private = "test" };
            _mockCryptoService.Setup(m => m.GenerateKeyPair()).Returns(newKeyPair);
            _mockConfigurationService.SetupGet(x => x.DeviceKeyPair).Returns(new KeyPair { Private = "test" });
            _subject.Execute().Wait();

            _mockConfigurationService.VerifySet(m => m.DeviceKeyPair = newKeyPair);
        }

        [Test]
        public void Execute_WhenConfigurationServiceContainsDeviceKeyPairWithEmptyPrivateKey_CreatesAKeyPair()
        {
            var newKeyPair = new KeyPair { Public = "test", Private = "test" };
            _mockCryptoService.Setup(m => m.GenerateKeyPair()).Returns(newKeyPair);
            _mockConfigurationService.SetupGet(m => m.DeviceKeyPair).Returns(new KeyPair { Public = "test" });
            _subject.Execute().Wait();

            _mockConfigurationService.VerifySet(m => m.DeviceKeyPair = newKeyPair);
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
