namespace OmniHolidays.MessagesWorkspace.MessageDetails
{
    using Caliburn.Micro;
    using OmniHolidays.MessagesWorkspace.ContactList;

    public interface IMessageDetailsHeaderViewModel : IScreen
    {
        IContactSource ContactsSource { get; set; }

        void SendMessage(string template);

        void Reset();
    }
}