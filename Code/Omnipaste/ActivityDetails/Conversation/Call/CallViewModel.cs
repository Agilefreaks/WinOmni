namespace Omnipaste.ActivityDetails.Conversation.Call
{
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Models;
    using Omnipaste.Services;

    public class CallViewModel : DetailsViewModelWithContact<Call>, ICallViewModel
    {
        #region Constructors and Destructors

        public CallViewModel(IUiRefreshService uiRefreshService)
            : base(uiRefreshService)
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