namespace OmniApi.Models
{
    public class Device
    {
        #region Constructors and Destructors

        public Device()
        {
        }

        public Device(string registrationId)
        {
            RegistrationId = registrationId;
        }

        #endregion

        #region Public Properties

        public string Name { get; set; }

        public string Provider { get; set; }

        public string PublicKey { get; set; }

        public string RegistrationId { get; set; }

        public string Id { get; set; }

        #endregion
    }
}