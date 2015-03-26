namespace Omnipaste.ContactList
{
    using OmniUI.List;

    public interface IContactListViewModel : IListViewModel<IContactInfoViewModel>
    {
        string FilterText { get; set; }

        bool CanSelectMultipleItems { get; set; }

        bool ShowStarred { get; set; }
    }
}