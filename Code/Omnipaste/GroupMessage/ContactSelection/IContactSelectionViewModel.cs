namespace Omnipaste.GroupMessage.ContactSelection
{
    using System.Collections.ObjectModel;
    using Omnipaste.ContactList;
    using Omnipaste.Presenters;
    using OmniUI.List;

    public interface IContactSelectionViewModel : IListViewModel<IContactInfoSelectionViewModel>
    {
        ObservableCollection<ContactInfoPresenter> SelectedContacts { get; set; }
    }
}