namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using OmniUI.Framework;

    public interface IContactSource
    {
        IDeepObservableCollectionView Contacts { get; }
    }
}