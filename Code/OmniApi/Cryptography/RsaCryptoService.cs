namespace OmniApi.Cryptography
{
    using System;
    using System.Collections.Generic;
    using OmniCommon.Models;
    using Org.BouncyCastle.Asn1;
    using Org.BouncyCastle.Crypto;
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

        private const int Certainty = 25;

        private const int KeySize = 1024;

        #endregion

        #region Public Methods and Operators

        public byte[] Decrypt(byte[] input, string privateKey)
        {
            var engine = new RsaEngine();
            engine.Init(false, PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey)));

            return TransformBytes(input, engine);
        }

        public byte[] Encrypt(byte[] input, string publicKey)
        {
            var engine = new RsaEngine();
            engine.Init(true, PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey)));

            return TransformBytes(input, engine);
        }

        public KeyPair GenerateKeyPair()
        {
            var keyGenerator = new RsaKeyPairGenerator();
            var param = new RsaKeyGenerationParameters(BigInteger.ValueOf(3), new SecureRandom(), KeySize, Certainty);
            keyGenerator.Init(param);
            var asymmetricCipherKeyPair = keyGenerator.GenerateKeyPair();

            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(asymmetricCipherKeyPair.Private);
            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(asymmetricCipherKeyPair.Public);

            return new KeyPair { Private = FormatKey(privateKeyInfo), Public = FormatKey(publicKeyInfo) };
        }

        #endregion

        #region Methods

        private static string FormatKey(IAsn1Convertible createPrivateKeyInfo)
        {
            return Convert.ToBase64String(createPrivateKeyInfo.ToAsn1Object().GetDerEncoded());
        }

        private static byte[] TransformBytes(byte[] input, IAsymmetricBlockCipher cipher)
        {
            var blockSize = cipher.GetInputBlockSize();
            var output = new List<byte>();

            for (var chunkPosition = 0; chunkPosition < input.Length; chunkPosition += blockSize)
            {
                var chunkSize = Math.Min(blockSize, input.Length - chunkPosition);
                output.AddRange(cipher.ProcessBlock(input, chunkPosition, chunkSize));
            }

            return output.ToArray();
        }

        #endregion
    }
}