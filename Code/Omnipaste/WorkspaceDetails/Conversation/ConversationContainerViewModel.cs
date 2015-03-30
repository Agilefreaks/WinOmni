namespace Omnipaste.WorkspaceDetails.Conversation
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Ninject;
    using Omnipaste.Presenters;
    using Omnipaste.SMSComposer;

    public class ConversationContainerViewModel : WorkspaceDetailsContentViewModel<ContactInfoPresenter>,
                                                  IConversationContainerViewModel
    {
        private ObservableCollection<ContactInfoPresenter> _recipients;

        [Inject]
        public ISMSComposerViewModel SMSComposer { get; set; }

        [Inject]
        public IConversationContentViewModel ConversationContentViewModel { get; set; }

        #region IConversationContainerViewModel Members

        public ObservableCollection<ContactInfoPresenter> Recipients
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
            base.OnDeactivate(close);
        }
    }
}