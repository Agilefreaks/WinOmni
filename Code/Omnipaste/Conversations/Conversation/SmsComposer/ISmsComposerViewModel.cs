namespace Omnipaste.Conversations.Conversation.SMSComposer
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Omnipaste.Framework.Models;

    public interface ISMSComposerViewModel : IScreen
    {
        string Message { get; set; }

        ObservableCollection<ContactModel> Recipients { get; set; }

        void Send();
    }
}