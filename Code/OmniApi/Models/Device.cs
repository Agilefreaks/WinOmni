namespace OmniApi.Models
{
    public class Device
    {
        #region Constructors and Destructors

        public Device()
        {
        }

        public Device(string identifier)
        {
            Identifier = identifier;
        }

        public Device(string identifier, string registratonId)
            : this(identifier)
        {
            RegistrationId = registratonId;
        }

        #endregion

        #region Public Properties

        public string Identifier { get; set; }

        public string Name { get; set; }

        public string Provider { get; set; }

        public string PublicKey { get; set; }

        public string RegistrationId { get; set; }

        #endregion
    }
}