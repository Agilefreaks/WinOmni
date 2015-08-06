namespace Omnipaste.Conversations.Conversation
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Caliburn.Micro;
    using Omnipaste.Framework.Models;
    using OmniUI.Attributes;
    using OmniUI.Details;
<<<<<<< HEAD:Code/Omnipaste/Conversations/Conversation/ConversationViewModel.cs
    using OmniUI.Framework.Models;
=======
    using OmniUI.Presenters;
>>>>>>> Fixes selection problems:Code/Omnipaste/WorkspaceDetails/Conversation/ConversationViewModel.cs

    [UseView(typeof(DetailsWithHeaderView))]
    public class ConversationViewModel : DetailsWithHeaderViewModelBase<IDetailsViewModel, IDetailsViewModel>, IConversationViewModel
    {
        private ObservableCollection<ContactModel> _recipients;

        public override IModel Model
        {
            get
            {
                return base.Model;
            }
            set
            {
                base.Model = value;
                HeaderViewModel.Model = value;
                ContentViewModel.Model = value;
            }
        }

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

        public override IPresenter Model
        {
            get
            {
                return base.Model;
            }
            set
            {
                base.Model = value;
                HeaderViewModel.Model = value;
                ContentViewModel.Model = value;
            }
        }

        public ConversationViewModel(
            IConversationHeaderViewModel headerViewModel,
            IConversationContainerViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            if (Recipients != null)
            {
                Recipients.CollectionChanged += RecipientsCollectionChanged;
            }
        }

        private void RecipientsCollectionChanged(
            object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Model = _recipients.Count == 1 ? _recipients.First() : null;
        }

        protected override void OnDeactivate(bool close)
        {
            if (((IConversationHeaderViewModel)HeaderViewModel).State == ConversationHeaderStateEnum.Deleted && !close)
            {
                ((IConductor)Parent).DeactivateItem(this, true);
            }
            else
            {
                if (Recipients != null)
                {
                    Recipients.CollectionChanged -= RecipientsCollectionChanged;
                }

                base.OnDeactivate(close);
            }
        }
    }
}