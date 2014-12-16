namespace OmniApi.Cryptography
{
    using OmniCommon.Models;

    public interface ICryptoService
    {
        KeyPair GenerateKeyPair();
    }
}
