namespace OmniApi.Cryptography
{
    public abstract class KeyPair
    {
        #region Public Properties

        public abstract string Private { get; }

        public abstract string Public { get; }

        #endregion
    }
}