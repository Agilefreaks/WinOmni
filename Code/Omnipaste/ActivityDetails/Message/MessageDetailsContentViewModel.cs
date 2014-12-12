namespace Omnipaste.ActivityDetails.Message
{
    using Ninject;
    using Omnipaste.SMSComposer;

    public class MessageDetailsContentViewModel : ActivityDetailsContentViewModel, IMessageDetailsContentViewModel
    {
        #region Public Properties

        [Inject]
        public IConversationViewModel ConversationViewModel { get; set; }

        [Inject]
        public IInlineSMSComposerViewModel SMSComposer { get; set; }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            ConversationViewModel.ContactInfo = Model.ExtraData.ContactInfo;
            ConversationViewModel.Activate();
            SMSComposer.ContactInfo = Model.ExtraData.ContactInfo;
            SMSComposer.Activate();
        }

        protected override void OnDeactivate(bool close)
        {
            ConversationViewModel.Deactivate(close);
            SMSComposer.Deactivate(close);
            base.OnDeactivate(close);
        }

        #endregion
    }
}