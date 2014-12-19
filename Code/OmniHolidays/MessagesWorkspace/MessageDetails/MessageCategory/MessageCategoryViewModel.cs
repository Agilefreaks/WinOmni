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

        private IList<object> _languages;

        private string _selectedLanguage;

        private string _selectedCategory;

        public MessageCategoryViewModel(IMessageDefinitionProvider messageDefinitionProvider)
        {
            _messageDefinitionProvider = messageDefinitionProvider;

            var knownLanguages = new Dictionary<string, string>
                                     {
                                         { "en", "English" },
                                         { "pt", "Portuguese" },
                                         { "pl", "Polish" },
                                         { "ro", "Romanian" }
                                     };
            SelectedLanguage = knownLanguages[CultureInfo.CurrentCulture.TwoLetterISOLanguageName];
        }

        public IList<object> Languages
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

            var resourceManager = new ResourceManager(typeof(Resources));
            _messageDefinitionProvider.Get()
                .Subscribe(
                    messages =>
                        {
                            Languages =
                                messages.Select(m => m.Language)
                                    .Distinct()
                                    .Select(lang => new { Value = lang, Content = resourceManager.GetString(lang) })
                                    .Cast<object>()
                                    .ToList();
                        },
                    () => { });
        }
    }
}
