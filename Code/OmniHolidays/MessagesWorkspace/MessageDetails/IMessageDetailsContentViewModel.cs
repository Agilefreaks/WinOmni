namespace OmniHolidays.MessagesWorkspace.MessageDetails
{
    using Caliburn.Micro;
    using OmniHolidays.MessagesWorkspace.ContactList;

    public interface IMessageDetailsContentViewModel : IScreen, IMessageWizardViewModel
    {
        void Reset();

        IContactSource ContactSource { get; set; }
    }
}