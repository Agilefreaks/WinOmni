namespace OmniApi.Dto
{
    public class DeviceDto
    {
        #region Constructors and Destructors

        public DeviceDto()
        {
        }

        public DeviceDto(string registrationId)
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