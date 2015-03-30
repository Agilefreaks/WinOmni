﻿namespace Omnipaste.WorkspaceDetails.Conversation
{
    using Caliburn.Micro;
    using OmniUI.Attributes;
    using OmniUI.Details;

    [UseView(typeof(DetailsViewWithHeader))]
    public class ConversationViewModel : DetailsViewModelWithHeaderBase<IWorkspaceDetailsHeaderViewModel, IWorkspaceDetailsContentViewModel>, IConversationViewModel
    {
        public ConversationViewModel(
            IConversationHeaderViewModel headerViewModel,
            IConversationContainerViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }

        protected override void OnDeactivate(bool close)
        {
            if (((IConversationHeaderViewModel)HeaderViewModel).State == ConversationHeaderStateEnum.Deleted && !close)
            {
                ((IConductor)Parent).DeactivateItem(this, true);
            }
            else
            {
                base.OnDeactivate(close);
            }
        }
    }
}