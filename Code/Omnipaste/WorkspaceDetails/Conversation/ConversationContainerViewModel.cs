namespace Omnipaste.WorkspaceDetails.Conversation
{
    using System.Collections.Generic;
    using Ninject;
    using Omnipaste.Presenters;
    using Omnipaste.SMSComposer;

    public class ConversationContainerViewModel : WorkspaceDetailsContentViewModel<ContactInfoPresenter>,
                                                  IConversationContainerViewModel
    {
        [Inject]
        public IConversationContentViewModel ConversationContentViewModel { get; set; }

        [Inject]
        public ISMSComposerViewModel SMSComposer { get; set; }

        protected override void OnActivate()
        {
            base.OnActivate();
            ConversationContentViewModel.Model = Model;
            ConversationContentViewModel.Activate();
            SMSComposer.Recipients = new List<ContactInfoPresenter> { Model };
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