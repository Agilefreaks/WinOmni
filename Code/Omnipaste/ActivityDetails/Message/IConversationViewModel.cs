namespace Omnipaste.ActivityDetails.Message
{
    using Caliburn.Micro;
    using Omnipaste.Activity.Models;

    public interface IConversationViewModel : IConductor, IParent<IMessageViewModel>, IActivate, IDeactivate
    {
        ContactInfo ContactInfo { get; set; }
    }
}