namespace Omnipaste.SMSComposer
{
    using Caliburn.Micro;
    using Omnipaste.Models;

    public interface ISMSComposerViewModel : IScreen
    {
        bool CanSend { get; }

        SMSMessage Model { get; set; }

        ContactInfo ContactInfo { get; set; }

        void Send();
    }
}