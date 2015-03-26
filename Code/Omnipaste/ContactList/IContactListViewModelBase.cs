namespace Omnipaste.ContactList
{
    using Omnipaste.Presenters;
    using OmniUI.Details;
    using OmniUI.List;

    public interface IContactListViewModel<T> : IListViewModel<T>
        where T : class, IDetailsViewModel<ContactInfoPresenter>
    {
        string FilterText { get; set; }

        bool CanSelectMultipleItems { get; set; }

        bool ShowStarred { get; set; }
    }
}