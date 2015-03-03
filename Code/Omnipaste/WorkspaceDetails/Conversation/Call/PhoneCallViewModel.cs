namespace Omnipaste.WorkspaceDetails.Conversation.PhoneCall
{
    using System;
    using OmniCommon.Interfaces;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Models;
    using Omnipaste.Services;

    public class PhoneCallViewModel : ConversationItemViewModel<PhoneCall>, IPhoneCallViewModel
    {
        #region Constructors and Destructors

        public PhoneCallViewModel(IUiRefreshService uiRefreshService, IConfigurationService configurationService)
            : base(uiRefreshService, configurationService)
        {
        }

        #endregion

        public string Title
        {
            get
            {
                return Model.Source == SourceType.Remote
                           ? Properties.Resources.IncommingCallLabel
                           : Properties.Resources.OutgoingCallLabel;
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