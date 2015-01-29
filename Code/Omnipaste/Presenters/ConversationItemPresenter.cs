namespace Omnipaste.Presenters
{
    using System;
    using Caliburn.Micro;
    using Omnipaste.Models;
    using Message = Omnipaste.Models.Message;

    public class ConversationItemPresenter : PropertyChangedBase
    {
        public IConversationItem BackingModel { get; private set; }

        public string Content { get; private set; }

        public DateTime Time { get; private set; }

        public string UniqueId { get; private set; }

        public ConversationItemPresenter(Call call)
        {
            BackingModel = call;
            Content = string.Empty;
            Time = call.Time;
            UniqueId = call.UniqueId;
        }

        public ConversationItemPresenter(Message message)
        {
            BackingModel = message;
            Content = message.Content;
            Time = message.Time;
            UniqueId = message.UniqueId;
        }
    }
}