namespace Omnipaste.WorkspaceDetails.Conversation.Call
{
    using OmniCommon.Interfaces;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Models;
    using Omnipaste.Services;

    public class CallViewModel : ConversationItemViewModel<Call>, ICallViewModel
    {
        #region Constructors and Destructors

        public CallViewModel(IUiRefreshService uiRefreshService, IConfigurationService configurationService)
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
    }
}