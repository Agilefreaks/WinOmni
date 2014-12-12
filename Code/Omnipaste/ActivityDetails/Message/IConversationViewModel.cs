namespace Omnipaste.ActivityDetails.Message
{
    using Caliburn.Micro;
    using Omnipaste.Models;

    public interface IConversationViewModel : IConductor, IParent<IScreen>, IActivate, IDeactivate
    {
        ContactInfo ContactInfo { get; set; }
    }
}