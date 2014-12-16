namespace OmniApiTests.Cryptography
{
    using System.Text;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniApi.Cryptography;

    [TestFixture]
    public class RsaCryptoServiceTests
    {
        private RsaCryptoService _subject;
        
        [SetUp]
        public void SetUp()
        {
            _subject = new RsaCryptoService();
        }

        [Test]
        public void GenerateKey_Always_GeneratesPublicKey()
        {
            _subject.GenerateKeyPair().Public.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void GenerateKey_Always_GeneratesPrivateKey()
        {
            _subject.GenerateKeyPair().Private.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Decrypt_DataSizeIsLessThanKeySize_ReturnsDecryptedItem()
        {
            const string Data = "Data";
            var keyPair = _subject.GenerateKeyPair();
            var encryptedData = _subject.Encrypt(Encoding.UTF8.GetBytes(Data), keyPair.Public);
            var decryptedData = Encoding.UTF8.GetString(_subject.Decrypt(encryptedData, keyPair.Private));

            decryptedData.Should().Be(Data);
        }

        [Test]
        public void Decrypt_DataSizeIsGreaterThanKeySize_ReturnsDecryptedItem()
        {
            var data = new string('D', 10000);
            var keyPair = _subject.GenerateKeyPair();
            var encryptedData = _subject.Encrypt(Encoding.UTF8.GetBytes(data), keyPair.Public);
            var decryptedData = Encoding.UTF8.GetString(_subject.Decrypt(encryptedData, keyPair.Private));

            decryptedData.Should().Be(data);
        }
    }
}
