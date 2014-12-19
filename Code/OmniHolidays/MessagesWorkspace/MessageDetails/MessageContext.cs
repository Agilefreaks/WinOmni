namespace OmniHolidays.MessagesWorkspace.MessageDetails
{
    using OmniUI.Framework;

    public class MessageContext
    {
        public string MessageCategory { get; set; }

        public string Language { get; set; }

        public string Template { get; set; }

        public IDeepObservableCollectionView Contacts { get; set; }
    }
}
