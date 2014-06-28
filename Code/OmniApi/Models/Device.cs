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
            this.identifier = identifier;
        }

        public Device(string identifier, string registratonId) :  this(identifier)
        {
            registration_id = registratonId;
        }

        #endregion

        #region Public Properties

        public string identifier { get; set; }

        public string name { get; set; }

        public string registration_id { get; set; }

        public string provider { get; set; }

        #endregion
    }
}