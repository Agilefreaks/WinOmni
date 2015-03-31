namespace Omnipaste.WorkspaceDetails.Conversation
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Omnipaste.Models;
    using OmniUI.Attributes;
    using OmniUI.Details;

    [UseView(typeof(DetailsViewWithHeader))]
    public class ConversationViewModel : DetailsViewModelWithHeaderBase<IWorkspaceDetailsHeaderViewModel, IWorkspaceDetailsContentViewModel>, IConversationViewModel
    {
        private ObservableCollection<ContactModel> _recipients;

        public ObservableCollection<ContactModel> Recipients
        {
            get
            {
                return _recipients;
            }
            set
            {
                if (_recipients == value)
                {
                    return;
                }

                _recipients = value;
                ((IConversationHeaderViewModel)HeaderViewModel).Recipients = _recipients;
                ((IConversationContainerViewModel)ContentViewModel).Recipients = _recipients;
                NotifyOfPropertyChange(() => Recipients);
            }
        }

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