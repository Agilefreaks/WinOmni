namespace Omnipaste.Models
{
    using Omnipaste.Helpers;
    using PhoneCalls.Models;

    public class PhoneCall : BaseModel, IConversationItem
    {
        #region Constructors and Destructors

        public PhoneCall()
        {
            ContactInfo = new ContactInfo();
            Content = string.Empty;
            Source = SourceType.Local;
        }

        public PhoneCall(PhoneCallDto phoneCallDto)
            : this()
        {
            Id = phoneCallDto.Id;
            Source = SourceType.Remote;
        }

        #endregion

        #region Public Properties

        public string Id { get; set; }

        public ContactInfo ContactInfo { get; set; }

        public SourceType Source { get; set; }

        public string Content { get; set; }

        #endregion
    }
}