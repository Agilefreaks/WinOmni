namespace OmniApi.Cryptography
{
    using System;
    using System.Collections.Generic;
    using OmniCommon.Models;
    using Org.BouncyCastle.Asn1;
    using Org.BouncyCastle.Crypto.Engines;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Math;
    using Org.BouncyCastle.Pkcs;
    using Org.BouncyCastle.Security;
    using Org.BouncyCastle.X509;

    public class RsaCryptoService : ICryptoService
    {
        #region Constants

        private const int Certainity = 25;

        private const int KeySize = 1024;

        #endregion

        #region Public Methods and Operators

        public byte[] Decrypt(byte[] input, string privateKey)
        {
            var engine = new RsaEngine();
            engine.Init(false, PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey)));

            var blockSize = engine.GetInputBlockSize();
            var output = new List<byte>();

            for (var chunkPosition = 0; chunkPosition < input.Length; chunkPosition += blockSize)
            {
                var chunkSize = Math.Min(blockSize, input.Length - chunkPosition);
                output.AddRange(engine.ProcessBlock(input, chunkPosition, chunkSize));
            }

            return output.ToArray();
        }

        public byte[] Encrypt(byte[] input, string publicKey)
        {
            var engine = new RsaEngine();
            engine.Init(true, PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey)));
            var blockSize = engine.GetInputBlockSize();

            var output = new List<byte>();
            for (var chunkPosition = 0; chunkPosition < input.Length; chunkPosition += blockSize)
            {
                var chunkSize = Math.Min(blockSize, input.Length - chunkPosition);
                output.AddRange(engine.ProcessBlock(input, chunkPosition, chunkSize));
            }
            return output.ToArray();
        }

        public KeyPair GenerateKeyPair()
        {
            var keyGenerator = new RsaKeyPairGenerator();
            var param = new RsaKeyGenerationParameters(BigInteger.ValueOf(3), new SecureRandom(), KeySize, Certainity);
            keyGenerator.Init(param);
            var asymmetricCipherKeyPair = keyGenerator.GenerateKeyPair();

            return new KeyPair
                       {
                           Private =
                               ConvertToString(
                                   PrivateKeyInfoFactory.CreatePrivateKeyInfo(asymmetricCipherKeyPair.Private)
                               .ToAsn1Object()),
                           Public =
                               ConvertToString(
                                   SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(
                                       asymmetricCipherKeyPair.Public).ToAsn1Object())
                       };
        }

        #endregion

        #region Methods

        private string ConvertToString(Asn1Object createSubjectPublicKeyInfo)
        {
            return Convert.ToBase64String(createSubjectPublicKeyInfo.GetDerEncoded());
        }

        #endregion
    }
}