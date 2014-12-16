namespace OmniCommon.Models
{
    using System;

    [Serializable]
    public class KeyPair
    {
        #region Public Properties

        public string Private { get; set; }

        public string Public { get; set; }

        #endregion
    }
}