namespace Omnipaste.Models
{
    using Omnipaste.Helpers;
    using SMS.Models;

    public abstract class SmsMessage : BaseModel, IConversationItem
    {
        #region Constructors and Destructors

        protected SmsMessage()
        {
        }

        protected SmsMessage(SmsMessageDto smsMessageDto) : this()
        {
            Id = smsMessageDto.Id;
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

        public abstract SourceType Source { get; }

        #endregion
    }
}