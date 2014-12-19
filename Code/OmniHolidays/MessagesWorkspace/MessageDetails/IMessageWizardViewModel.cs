namespace OmniHolidays.MessagesWorkspace.MessageDetails
{
    public interface IMessageWizardViewModel
    {
        MessageContext CurrentMessageContext { get; }

        void GoToNextStep();

        void GoToPreviousStep();
    }
}