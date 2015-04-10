namespace Omnipaste.WorkspaceDetails.Conversation.PhoneCall
{
    using System;
    using OmniCommon.Interfaces;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using Omnipaste.Properties;
    using Omnipaste.Services;

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