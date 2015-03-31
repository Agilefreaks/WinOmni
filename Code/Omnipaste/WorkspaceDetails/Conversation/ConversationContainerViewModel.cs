namespace Omnipaste.WorkspaceDetails.Conversation
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using Ninject;
    using Omnipaste.Models;
    using Omnipaste.SMSComposer;

    public class ConversationContainerViewModel : WorkspaceDetailsContentViewModel<ContactModel>,
                                                  IConversationContainerViewModel
    {
        private ObservableCollection<ContactModel> _recipients;

        [Inject]
        public ISMSComposerViewModel SMSComposer { get; set; }

        [Inject]
        public IConversationContentViewModel ConversationContentViewModel { get; set; }

        #region IConversationContainerViewModel Members

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
                _recipients.CollectionChanged -= RecipientsOnCollectionChanged;
                _recipients.CollectionChanged += RecipientsOnCollectionChanged;
                SMSComposer.Recipients = _recipients;
                NotifyOfPropertyChange(() => Recipients);
            }
        }

        #endregion

        protected override void OnActivate()
        {
            base.OnActivate();
            ConversationContentViewModel.Model = Model;
            ConversationContentViewModel.Activate();
            SMSComposer.Activate();
        }

        protected override void OnDeactivate(bool close)
        {
            ConversationContentViewModel.Deactivate(close);
            SMSComposer.Deactivate(close);
            if (_recipients != null)
            {
                _recipients.CollectionChanged -= RecipientsOnCollectionChanged;
            }
            base.OnDeactivate(close);
        }

        private void RecipientsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var initialModel = ConversationContentViewModel.Model;
            ConversationContentViewModel.Model = _recipients.Count >= 2 ? null : Model;

            if (initialModel != ConversationContentViewModel.Model)
            {
                ConversationContentViewModel.RefreshConversation();
            }
        }
    }
}