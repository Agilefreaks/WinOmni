namespace OmniHolidays.MessagesWorkspace.MessageDetails.MessageCategory
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Resources;
    using OmniHolidays.Properties;
    using OmniHolidays.Providers;

    public class MessageCategoryViewModel : MessageStepViewModelBase
    {
        private readonly IMessageDefinitionProvider _messageDefinitionProvider;

        private IList<LanguageInfo> _languages;

        private string _selectedLanguage;

        public MessageCategoryViewModel(IMessageDefinitionProvider messageDefinitionProvider)
        {
            _messageDefinitionProvider = messageDefinitionProvider;

            Languages = new List<LanguageInfo>();
        }

        public IList<LanguageInfo> Languages
        {
            get
            {
                return _languages;
            }
            set
            {
                if (Equals(value, _languages))
                {
                    return;
                }
                _languages = value;
                NotifyOfPropertyChange(() => Languages);
            }
        }

        public string SelectedLanguage
        {
            get
            {
                return _selectedLanguage;
            }
            set
            {
                if (value == _selectedLanguage)
                {
                    return;
                }
                _selectedLanguage = value;
                NotifyOfPropertyChange(() => SelectedLanguage);
            }
        }

        public void CategorySelected(string category)
        {
            MessageContext.MessageCategory = category;
            MessageContext.Language = SelectedLanguage;

            NotifyOnNext();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            if (Languages.Any())
            {
                return;
            }

            _messageDefinitionProvider.Get()
                .Subscribe(
                    OnMessagesReceived,
                    () => { });
        }

        private void OnMessagesReceived(IList<MessageDefinition> messages)
        {
            var resourceManager = new ResourceManager(typeof(Resources));

            Languages = messages.Select(m => m.Language)
                    .Distinct()
                    .Select(lang => new LanguageInfo { Value = lang, Content = resourceManager.GetString(lang) })
                    .ToList();

            var knownLanguages = new Dictionary<string, string>
                                     {
                                         { "en", "English" },
                                         { "pt", "Portuguese" },
                                         { "pl", "Polish" },
                                         { "ro", "Romanian" }
                                     };
            var appLanguageName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            SelectedLanguage = HasMessagesForAppLanguage(knownLanguages, appLanguageName)
                                   ? knownLanguages[appLanguageName]
                                   : Languages.Select(l => l.Value).FirstOrDefault();
        }

        private bool HasMessagesForAppLanguage(Dictionary<string, string> knownLanguages, string appLanguageName)
        {
            return knownLanguages.ContainsKey(appLanguageName)
                   && Languages.Any(l => l.Value == knownLanguages[appLanguageName]);
        }
    }
}
