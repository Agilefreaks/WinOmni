namespace OmniApi.Cryptography
{
    using System;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Pkcs;
    using Org.BouncyCastle.X509;

    public class RsaKeyPair : KeyPair
    {
        #region Fields

        private readonly AsymmetricCipherKeyPair _asymmetricCipherKeyPair;

        #endregion

        #region Constructors and Destructors

        public RsaKeyPair(AsymmetricCipherKeyPair asymmetricCipherKeyPair)
        {
            _asymmetricCipherKeyPair = asymmetricCipherKeyPair;
        }

        #endregion

        #region Public Properties

        public override string Private
        {
            get
            {
                return
                    Convert.ToBase64String(
                        PrivateKeyInfoFactory.CreatePrivateKeyInfo(_asymmetricCipherKeyPair.Private)
                            .ToAsn1Object()
                            .GetDerEncoded());
            }
        }

        public override string Public
        {
            get
            {
                return
                    Convert.ToBase64String(
                        SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(_asymmetricCipherKeyPair.Public)
                            .ToAsn1Object()
                            .GetDerEncoded());
            }
        }

        #endregion
    }
}