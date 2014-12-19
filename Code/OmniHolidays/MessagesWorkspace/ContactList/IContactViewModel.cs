namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using OmniUI.Details;
    using OmniUI.Presenters;

    public interface IContactViewModel : IDetailsViewModel<IContactInfoPresenter>
    {
        bool IsSelected { get; set; }
    }
}