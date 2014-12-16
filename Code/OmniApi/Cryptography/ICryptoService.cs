namespace OmniApi.Cryptography
{
    using OmniCommon.Models;

    public interface ICryptoService
    {
        KeyPair GenerateKeyPair();

        byte[] Encrypt(byte[] input, string publicKey);

        byte[] Decrypt(byte[] input, string privateKey);
    }
}
