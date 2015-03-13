namespace Omnipaste.GroupMessage
{
    using Caliburn.Micro;
    using Omnipaste.GroupMessage.ContactSelection;
    using Omnipaste.WorkspaceDetails.Conversation;

    public class GroupMessageViewModel : Screen, IGroupMessageViewModel
    {
        private readonly IContactSelectionViewModel _contactSelection;

        private readonly IConversationViewModel _conversation;

        public IContactSelectionViewModel ContactSelection
        {
            get
            {
                return _contactSelection;
            }
        }

        public IConversationViewModel Conversation
        {
            get
            {
                return _conversation;
            }
        }

        public GroupMessageViewModel(IContactSelectionViewModel contactSelectionViewModel, IConversationViewModel conversationViewModel)
        {
            _contactSelection = contactSelectionViewModel;
            _conversation = conversationViewModel;
        }
    }
}