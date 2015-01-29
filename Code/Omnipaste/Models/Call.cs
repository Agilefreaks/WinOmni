namespace Omnipaste.Models
{
    using Omnipaste.Helpers;
    using OmniUI.Models;
    using PhoneCalls.Models;

    public class Call : BaseModel, IConversationItem
    {
        #region Constructors and Destructors

        public Call()
        {
            ContactInfo = new ContactInfo();
            Content = string.Empty;
            Source = SourceType.Local;
        }

        public Call(PhoneCall phoneCall)
            : this()
        {
            string firstName, lastName;
            NameParser.Parse(phoneCall.ContactName, out firstName, out lastName);
            Id = phoneCall.Id;
            Source = SourceType.Remote;
            ContactInfo = new ContactInfo
                              {
                                  FirstName = firstName, 
                                  LastName = lastName, 
                                  Phone = phoneCall.Number
                              };
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