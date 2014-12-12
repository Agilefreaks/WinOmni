namespace Omnipaste.ActivityDetails.Message
{
    using Caliburn.Micro;
    using Omnipaste.Models;

    public interface IConversationViewModel : IConductor, IParent<IMessageViewModel>, IActivate, IDeactivate
    {
        ContactInfo ContactInfo { get; set; }
    }
}