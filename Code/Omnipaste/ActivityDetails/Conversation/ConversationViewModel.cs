namespace Omnipaste.ActivityDetails.Conversation
{
    using OmniUI.Attributes;

    [UseView("OmniUI.Details.DetailsViewWithHeader", IsFullyQualifiedName = true)]
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