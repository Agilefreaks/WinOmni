namespace Omnipaste.Models
{
    using Omnipaste.Helpers;
    using SMS.Models;

    public class SmsMessage : BaseModel, IConversationItem
    {
        #region Constructors and Destructors

        public SmsMessage()
        {
            ContactInfo = new ContactInfo();
            Content = string.Empty;
            Source = SourceType.Local;
        }

        public SmsMessage(SmsMessageDto smsMessageDto)
            : this()
        {
            Id = smsMessageDto.Id;
            Source = SourceType.Remote;
            Content = smsMessageDto.Content;
            string firstName, lastName;
            NameParser.Parse(smsMessageDto.ContactName, out firstName, out lastName);
            ContactInfo = new ContactInfo
                              {
                                  FirstName = firstName,
                                  LastName = lastName,
                                  Phone = smsMessageDto.PhoneNumber
                              };
        }

        #endregion

        #region Public Properties

        public string Id { get; set; }

        public ContactInfo ContactInfo { get; set; }

        public string Content { get; set; }

        public SourceType Source { get; set; }

        #endregion
    }
}