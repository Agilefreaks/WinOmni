namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using Caliburn.Micro;
    using OmniUI.Presenters;

    public interface IContactListContentViewModel : IScreen, IContactSource
    {
        IObservableCollection<IContactInfoPresenter> Items { get; }
    }
}