namespace Omnipaste.ContactList
{
    using Omnipaste.Presenters;
    using OmniUI.Details;

    public interface IContactInfoSelectionViewModel : IDetailsViewModel<ContactInfoPresenter>
    {
        bool IsSelected { get; set; }
    }
}