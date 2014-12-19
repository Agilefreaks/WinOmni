namespace Omnipaste.ActivityDetails.Conversation
{
    using Caliburn.Micro;
    using Omnipaste.Models;
    using OmniUI.Models;

    public interface IConversationContentViewModel : IConductor, IParent<IScreen>, IActivate, IDeactivate
    {
        ContactInfo ContactInfo { get; set; }
    }
}