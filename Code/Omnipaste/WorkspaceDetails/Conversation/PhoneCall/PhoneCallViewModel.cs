namespace Omnipaste.WorkspaceDetails.Conversation.PhoneCall
{
    using System;
    using OmniCommon.Interfaces;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Properties;
    using Omnipaste.Services;

    public class PhoneCallViewModel : ConversationItemViewModel<PhoneCallPresenter>, IPhoneCallViewModel
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