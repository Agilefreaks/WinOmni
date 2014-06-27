namespace OmniApi.Models
{
    using System;

    public class Device
    {
        #region Public Properties

        public DateTime CreatedAt { get; set; }

        public string Identifier { get; set; }

        public string Name { get; set; }

        public string RegistrationId { get; set; }

        public DateTime UpdatedAt { get; set; }

        #endregion
    }
}