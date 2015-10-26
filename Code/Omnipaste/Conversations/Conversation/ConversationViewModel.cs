namespace Omnipaste.Conversations.Conversation
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using OmniUI.Attributes;
    using OmniUI.Details;
    using OmniUI.Framework.Models;

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

                UpdateRecipientsHooks(_recipients, value);
                _recipients = value;
                ConversationHeaderViewModel.Recipients = _recipients;
                ConversationContainerViewModel.Recipients = _recipients;
                NotifyOfPropertyChange(() => Recipients);
            }
        }

        public IConversationHeaderViewModel ConversationHeaderViewModel { get; private set; }

        public IConversationContainerViewModel ConversationContainerViewModel { get; private set; }

        public ConversationViewModelStateEnum State { get; set; }

        public ConversationViewModel(
            IConversationHeaderViewModel headerViewModel,
            IConversationContainerViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
            ConversationHeaderViewModel = headerViewModel;
            ConversationContainerViewModel = contentViewModel;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            if (Recipients == null)
            {
                Recipients = new ObservableCollection<ContactModel>();
            }
        }

        protected override void OnDeactivate(bool close)
        {
            if (Recipients != null)
            {
                Recipients.CollectionChanged -= RecipientsCollectionChanged;
            }
            State = ConversationHeaderViewModel.State == ConversationHeaderStateEnum.Deleted
                        ? ConversationViewModelStateEnum.Deleted
                        : ConversationViewModelStateEnum.Normal;
            base.OnDeactivate(close);
        }

        private void UpdateRecipientsHooks(INotifyCollectionChanged oldValue, INotifyCollectionChanged newValue)
        {
            if (oldValue != null)
            {
                oldValue.CollectionChanged -= RecipientsCollectionChanged;
            }

            if (newValue != null && IsActive)
            {
                newValue.CollectionChanged += RecipientsCollectionChanged;
            }
        }

        private void RecipientsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Model = _recipients.Count == 1 ? _recipients.First() : null;
        }
    }
}