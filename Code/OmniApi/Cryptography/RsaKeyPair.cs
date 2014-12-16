namespace OmniApi.Cryptography
{
    using System;
    using OmniCommon.Models;
    using Org.BouncyCastle.Asn1;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Pkcs;
    using Org.BouncyCastle.X509;

    [Serializable]
    public class RsaKeyPair : KeyPair
    {
        #region Constructors and Destructors

        public RsaKeyPair(AsymmetricCipherKeyPair asymmetricCipherKeyPair)
        {
            Private =
                ConvertToString(
                    PrivateKeyInfoFactory.CreatePrivateKeyInfo(asymmetricCipherKeyPair.Private).ToAsn1Object());
            Public =
                ConvertToString(
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(asymmetricCipherKeyPair.Public)
                        .ToAsn1Object());
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