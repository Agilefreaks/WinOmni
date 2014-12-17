namespace OmniApi.Support.Converters
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.Practices.ServiceLocation;
    using Newtonsoft.Json;
    using OmniApi.Cryptography;
    using OmniCommon.Interfaces;

    public class EncryptionConverter : JsonConverter
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly ICryptoService _cryptoService;

        #endregion

        #region Constructors and Destructors

        public EncryptionConverter()
        {
            _configurationService = ServiceLocator.Current.GetInstance<IConfigurationService>();
            _cryptoService = ServiceLocator.Current.GetInstance<ICryptoService>();
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var decryptedData = _cryptoService.Decrypt(
                Convert.FromBase64String((string)reader.Value),
                _configurationService.DeviceKeyPair.Private);
            var decryptedTextReader = new StringReader(Encoding.UTF8.GetString(decryptedData));

            return serializer.Deserialize(decryptedTextReader, objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // No external PublicKey
            throw new NotImplementedException();
        }

        #endregion
    }
}