namespace Omnipaste.Conversations.ContactList
{
    using System.Collections.ObjectModel;
    using Omnipaste.Conversations.ContactList.Contact;
    using Omnipaste.Framework.Models;
    using OmniUI.List;

    public interface IContactListViewModel : IListViewModel<IContactViewModel>
    {
        string FilterText { get; set; }

        bool CanSelectMultipleItems { get; set; }

        bool ShowStarred { get; set; }

        ObservableCollection<ContactModel> SelectedContacts { get; }
    }
}