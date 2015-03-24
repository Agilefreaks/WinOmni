namespace Omnipaste.GroupMessage.ContactSelection
{
    using System.Collections.ObjectModel;
    using Omnipaste.ContactList;
    using Omnipaste.Presenters;

    public interface IContactSelectionViewModel : IContactListViewModel<IContactInfoSelectionViewModel>
    {
        ObservableCollection<ContactInfoPresenter> SelectedContacts { get; set; }

        ContactInfoPresenter PendingContact { get; set; }

        void AddPendingContact();
    }
}