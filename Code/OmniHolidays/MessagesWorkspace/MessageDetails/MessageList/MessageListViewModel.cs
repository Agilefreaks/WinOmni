namespace OmniHolidays.MessagesWorkspace.MessageDetails.MessageList
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OmniCommon;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Interfaces;
    using OmniHolidays.Providers;

    public class MessageListViewModel : MessageStepViewModelBase
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly IMessageDefinitionProvider _messageDefinitionProvider;

        private IList<MessageDefinition> _messageDefinitions;

        private IDisposable _settingsChangedSubscription;

        #endregion

        #region Constructors and Destructors

        public MessageListViewModel(
            IMessageDefinitionProvider messageDefinitionProvider,
            IConfigurationService configurationService)
        {
            _messageDefinitionProvider = messageDefinitionProvider;
            _configurationService = configurationService;
        }

        #endregion

        #region Public Properties

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

        #endregion

        #region Public Methods and Operators

        public void TemplateSelected(MessageDefinition messageDefinition)
        {
            MessageContext.Template = messageDefinition.MessageTemplate;

            NotifyOnNext();
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            UpdateMessageDefinitions();
            DisposeSettingsChangedSubscription();
            _settingsChangedSubscription =
                _configurationService.SettingsChangedObservable.SubscribeToSettingChange<bool>(
                    ConfigurationProperties.SMSSuffixEnabled,
                    _ => UpdateMessageDefinitions());
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            DisposeSettingsChangedSubscription();
        }

        private void DisposeSettingsChangedSubscription()
        {
            if (_settingsChangedSubscription == null)
            {
                return;
            }
            _settingsChangedSubscription.Dispose();
            _settingsChangedSubscription = null;
        }

        private void UpdateMessageDefinitions()
        {
            _messageDefinitionProvider.Get()
                .RunToCompletion(
                    messages =>
                        {
                            MessageDefinitions =
                                messages.Where(
                                    m =>
                                    m.Category == MessageContext.MessageCategory
                                    && m.Language == MessageContext.Language).ToList();
                        });
        }

        #endregion
    }
}