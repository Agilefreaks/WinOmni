namespace OmniHolidays.MessagesWorkspace.MessageDetails.MessageList
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OmniHolidays.Providers;

    public class MessageListViewModel : MessageStepViewModelBase
    {
        private readonly IMessageDefinitionProvider _messageDefinitionProvider;

        private IList<MessageDefinition> _messageDefinitions;

        public MessageListViewModel(IMessageDefinitionProvider messageDefinitionProvider)
        {
            _messageDefinitionProvider = messageDefinitionProvider;
        }

        public IList<MessageDefinition> MessageDefinitions
        {
            get
            {
                return _messageDefinitions;
            }
            set
            {
                if (Equals(value, _messageDefinitions))
                {
                    return;
                }
                _messageDefinitions = value;
                NotifyOfPropertyChange(() => MessageDefinitions);
            }
        }

        public void TemplateSelected(MessageDefinition messageDefinition)
        {
            MessageContext.Template = messageDefinition.MessageTemplate;

            NotifyOnNext();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            _messageDefinitionProvider.Get().Subscribe(
                messages =>
                    {
                        MessageDefinitions =
                            messages.Where(
                                m =>
                                m.Category == MessageContext.MessageCategory && m.Language == MessageContext.Language)
                                .ToList();
                    },
                () => { });
        }
    }
}