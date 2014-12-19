namespace OmniHolidays.MessagesWorkspace.MessageDetails.MessageCategory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Resources;
    using OmniHolidays.Providers;
    using global::OmniHolidays.Properties;

    public class MessageCategoryViewModel : MessageStepViewModelBase
    {
        private readonly IMessageDefinitionProvider _messageDefinitionProvider;

        private IList<object> _languages;

        public MessageCategoryViewModel(IMessageDefinitionProvider messageDefinitionProvider)
        {
            _messageDefinitionProvider = messageDefinitionProvider;
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
