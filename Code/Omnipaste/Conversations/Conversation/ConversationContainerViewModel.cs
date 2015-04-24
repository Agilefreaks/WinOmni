namespace Omnipaste.Conversations.Conversation
{
    using System.Collections.ObjectModel;
    using Ninject;
    using Omnipaste.Conversations.Conversation.SMSComposer;
    using Omnipaste.Framework.Models;
    using OmniUI.Details;

    public class ConversationContainerViewModel : DetailsViewModelBase<ContactModel>,
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
                SMSComposer.Recipients = _recipients;
                NotifyOfPropertyChange(() => Recipients);
            }
        }

        public override ContactModel Model
        {
            get
            {
                return base.Model;
            }
            set
            {
                base.Model = value;
                ConversationContentViewModel.Model = value;
            }
        }

        #endregion

        protected override void OnActivate()
        {
            base.OnActivate();
            ConversationContentViewModel.Activate();
            SMSComposer.Activate();
        }

        protected override void OnDeactivate(bool close)
        {
            ConversationContentViewModel.Deactivate(close);
            SMSComposer.Deactivate(close);
            
            base.OnDeactivate(close);
        }
    }
}