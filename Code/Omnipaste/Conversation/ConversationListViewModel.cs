namespace Omnipaste.Conversation
{
    using Omnipaste.Presenters;
    using OmniUI.List;

    public class ConversationListViewModel : ListViewModelBase<ConversationPresenter, IConversationInfoViewModel>, IConversationListViewModel
    {
        private readonly IConversationInfoViewModelFactory _conversationInfoViewModelFactory;

        private bool _showStarred;

        public ConversationListViewModel(IConversationInfoViewModelFactory conversationInfoViewModelFactory)
        {
            _conversationInfoViewModelFactory = conversationInfoViewModelFactory;
        }

        public bool ShowStarred
        {
            get
            {
                return _showStarred;
            }
            set
            {
                if (value.Equals(_showStarred))
                {
                    return;
                }
                _showStarred = value;
                NotifyOfPropertyChange();
                RefreshItems();
            }
        }

        protected override IConversationInfoViewModel CreateViewModel(ConversationPresenter model)
        {
            return _conversationInfoViewModelFactory.Create(model);
        }
    }
}