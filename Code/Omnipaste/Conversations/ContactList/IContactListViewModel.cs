namespace Omnipaste.Conversations.ContactList
{
    using Omnipaste.Conversations.ContactList.Contact;
    using OmniUI.List;

    public interface IContactListViewModel : IListViewModel<IContactViewModel>
    {
        string FilterText { get; set; }

        bool CanSelectMultipleItems { get; set; }

        bool ShowStarred { get; set; }
    }
}