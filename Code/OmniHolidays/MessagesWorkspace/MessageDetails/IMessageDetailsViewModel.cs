namespace OmniHolidays.MessagesWorkspace.MessageDetails
{
    using OmniHolidays.MessagesWorkspace.ContactList;
    using OmniUI.Details;

    public interface IMessageDetailsViewModel :
        IDetailsViewModelWithHeader<IMessageDetailsHeaderViewModel, IMessageDetailsContentViewModel>
    {
        IContactSource ContactsSource { get; set; }
    }
}