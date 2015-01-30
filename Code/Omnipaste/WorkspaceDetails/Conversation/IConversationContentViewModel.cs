namespace Omnipaste.WorkspaceDetails.Conversation
{
    using Caliburn.Micro;
    using Omnipaste.Models;

    public interface IConversationContentViewModel : IConductor, IActivate, IDeactivate
    {
        ContactInfo ContactInfo { get; set; }
    }
}