namespace OmniCommon.Models
{
    using System;

    [Serializable]
    public class DeviceInfo
    {
        #region Public Properties

        public string Identifier { get; set; }

        public string Name { get; set; }

        public string Provider { get; set; }

        public string PublicKey { get; set; }

        #endregion
    }
}