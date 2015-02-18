namespace Omnipaste.WorkspaceDetails.Conversation
{
    using Ninject;
    using Omnipaste.Presenters;
    using Omnipaste.SMSComposer;

    public class ConversationContainerViewModel : WorkspaceDetailsContentViewModel<ContactInfoPresenter>, IConversationContainerViewModel
    {
        #region Public Properties

        [Inject]
        public IConversationContentViewModel ConversationContentViewModel { get; set; }

        [Inject]
        public ISMSComposerViewModel SMSComposer { get; set; }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            ConversationContentViewModel.Model = Model;
            ConversationContentViewModel.Activate();
            SMSComposer.ContactInfo = Model.ContactInfo;
            SMSComposer.Activate();
        }

        protected override void OnDeactivate(bool close)
        {
            ConversationContentViewModel.Deactivate(close);
            SMSComposer.Deactivate(close);
            base.OnDeactivate(close);
        }

        #endregion
    }
}