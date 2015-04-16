namespace Omnipaste.Conversations.Conversation.PhoneCall
{
    using System;
    using OmniCommon.Interfaces;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using Omnipaste.Framework.Services;
    using Omnipaste.Properties;
    using OmniUI.Framework.Services;

    public class PhoneCallViewModel : ConversationItemViewModel<PhoneCallModel>, IPhoneCallViewModel
    {

        public PhoneCallViewModel(IUiRefreshService uiRefreshService, IConfigurationService configurationService)
            : base(uiRefreshService, configurationService)
        {
        }

        public string Title
        {
            get
            {
                return Model.Source == SourceType.Remote
                           ? Resources.IncommingCallLabel
                           : Resources.OutgoingCallLabel;
            }
        }

        public DateTime Time
        {
            get
            {
                return Model.Time;
            }
        }
    }
}