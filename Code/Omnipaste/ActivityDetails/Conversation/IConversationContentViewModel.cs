namespace Omnipaste.ActivityDetails.Conversation
{
    using Caliburn.Micro;
    using OmniUI.Models;

    public interface IConversationContentViewModel : IConductor, IActivate, IDeactivate
    {
        ContactInfo ContactInfo { get; set; }
    }
}