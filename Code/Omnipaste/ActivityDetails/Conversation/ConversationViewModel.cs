namespace Omnipaste.ActivityDetails.Conversation
{
    using OmniUI.Attributes;

    [UseView(typeof(OmniUI.Details.DetailsViewWithHeader))]
    public class ConversationViewModel : ActivityDetailsViewModel, IConversationViewModel
    {
        #region Constructors and Destructors

        public ConversationViewModel(
            IConversationHeaderViewModel headerViewModel,
            IConversationContainerViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }

        #endregion
    }
}