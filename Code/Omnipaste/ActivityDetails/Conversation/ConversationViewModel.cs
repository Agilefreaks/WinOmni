namespace Omnipaste.ActivityDetails.Conversation
{
    using Caliburn.Micro;
    using OmniUI.Attributes;
    using OmniUI.Details;

    [UseView(typeof(DetailsViewWithHeader))]
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

        protected override void OnDeactivate(bool close)
        {
            if (((IConversationHeaderViewModel)HeaderViewModel).State == ConversationHeaderStateEnum.Deleted)
            {
                if (!close)
                {
                    var parentConductor = Parent as IConductor;
                    if (parentConductor != null)
                    {
                        parentConductor.DeactivateItem(this, true);
                    }
                }
            }
            else
            {
                base.OnDeactivate(close);
            }
        }

    }
}