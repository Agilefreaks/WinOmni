namespace Omnipaste.SMSComposer
{
    using Caliburn.Micro;
    using Omnipaste.Models;

    public interface ISMSComposerViewModel : IScreen
    {
        ContactInfo ContactInfo { get; set; }

        string Message { get; set; }

        void Send();
    }
}