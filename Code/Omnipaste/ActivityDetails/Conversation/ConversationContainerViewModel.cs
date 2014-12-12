namespace Omnipaste.ActivityDetails.Conversation
{
    using Ninject;
    using Omnipaste.SMSComposer;

    public class ConversationContainerViewModel : ActivityDetailsContentViewModel, IConversationContainerViewModel
    {
        #region Public Properties

        [Inject]
        public IConversationContentViewModel ConversationContentViewModel { get; set; }

        [Inject]
        public IInlineSMSComposerViewModel SMSComposer { get; set; }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            ConversationContentViewModel.ContactInfo = Model.ExtraData.ContactInfo;
            ConversationContentViewModel.Activate();
            SMSComposer.ContactInfo = Model.ExtraData.ContactInfo;
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