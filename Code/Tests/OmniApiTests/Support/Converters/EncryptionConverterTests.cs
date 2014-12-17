namespace OmniApiTests.Support.Converters
{
    using System;
    using System.Text;
    using CommonServiceLocator.NinjectAdapter.Unofficial;
    using FluentAssertions;
    using Microsoft.Practices.ServiceLocation;
    using Moq;
    using Newtonsoft.Json;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniApi.Cryptography;
    using OmniApi.Support.Converters;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    [TestFixture]
    public class EncryptionConverterTests
    {
        private MoqMockingKernel _kernel;

        private Mock<IConfigurationService> _mockConfigurationService;

        private RsaCryptoService _cryptoService;

        private KeyPair _keyPair;

        public class Nested
        {
            public string Data { get; set; }
        }

        public class DataContainer
        {
            [JsonConverter(typeof(EncryptionConverter))]
            public Nested Nested { get; set; }
        }

        [SetUp]
        public void SetUp()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            _cryptoService = new RsaCryptoService();
            _keyPair = _cryptoService.GenerateKeyPair();
            _mockConfigurationService.SetupGet(m => m.DeviceKeyPair).Returns(_keyPair);
            _kernel = new MoqMockingKernel();
            _kernel.Bind<IConfigurationService>().ToConstant(_mockConfigurationService.Object);
            _kernel.Bind<ICryptoService>().ToConstant(_cryptoService);

            var serviceLocatorAdapter = new NinjectServiceLocator(_kernel);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorAdapter);
        }

        [Test]
        public void Deserialize_Always_DecryptsData()
        {
            const string Data = "Test 42";
            var nestedJson = JsonConvert.SerializeObject(new { Data });
            var encryptedNestedJson =
                Convert.ToBase64String(_cryptoService.Encrypt(Encoding.UTF8.GetBytes(nestedJson), _keyPair.Public));
            var x = JsonConvert.SerializeObject(new { Nested = encryptedNestedJson });

            var deserializedObject = JsonConvert.DeserializeObject<DataContainer>(x);
            
            deserializedObject.Nested.Data.Should().Be(Data);
        }
    }
}
