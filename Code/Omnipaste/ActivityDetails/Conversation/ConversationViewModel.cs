namespace Omnipaste.ActivityDetails.Conversation
{
    using OmniUI.Attributes;

    [UseView("Omnipaste.ActivityDetails.ActivityDetailsView", IsFullyQualifiedName = true)]
    public class ConversationViewModel : ActivityDetailsViewModel, IConversationViewModel
    {
        #region Constructors and Destructors

        public ConversationViewModel(
            IConversationHeaderViewModel headerViewModel,
            IConversationContainerViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            HeaderViewModel.Activate();
            ContentViewModel.Activate();
        }

        protected override void OnDeactivate(bool close)
        {
            HeaderViewModel.Deactivate(close);
            ContentViewModel.Deactivate(close);
            base.OnDeactivate(close);
        }

        #endregion
    }
}