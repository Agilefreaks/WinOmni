namespace Omnipaste.Models
{
    using Omnipaste.Helpers;
    using SMS.Models;

    public class Message : BaseModel, IConversationItem
    {
        #region Constructors and Destructors

        public Message()
        {
            ContactInfo = new ContactInfo();
            Content = string.Empty;
            Source = SourceType.Local;
        }

        public Message(SmsMessage smsMessage)
            : this()
        {
            Id = smsMessage.Id;
            Source = SourceType.Remote;
            Content = smsMessage.Content;
            string firstName, lastName;
            NameParser.Parse(smsMessage.ContactName, out firstName, out lastName);
            ContactInfo = new ContactInfo
                              {
                                  FirstName = firstName,
                                  LastName = lastName,
                                  Phone = smsMessage.PhoneNumber
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