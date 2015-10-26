namespace Omnipaste.Conversations
{
    using System.Collections.Specialized;
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.Conversations.ContactList;
    using Omnipaste.Conversations.Conversation;
    using Omnipaste.Framework;
    using Omnipaste.Properties;
    using OmniUI.Attributes;
    using OmniUI.Workspaces;

    [UseView(typeof(WorkspaceView))]
    public class ConversationWorkspace : MasterDetailsWorkspace, IConversationWorkspace
    {
        #region Constructors and Destructors

        public ConversationWorkspace(IContactListViewModel masterScreen, IDetailsConductorViewModel detailsConductor)
            : base(masterScreen, detailsConductor)
        {
            ContactListViewModel = masterScreen;
        }

        #endregion

        #region Public Properties

        public override string DisplayName
        {
            get
            {
                return Resources.People;
            }
        }

        public IContactListViewModel ContactListViewModel { get; private set; }

        [Inject]
        public IDetailsViewModelFactory DetailsViewModelFactory { get; set; }

        #endregion

        protected override void OnActivate()
        {
            base.OnActivate();
            ContactListViewModel.SelectedContacts.CollectionChanged += SelectedContactsCollectionChanged;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            ContactListViewModel.SelectedContacts.CollectionChanged -= SelectedContactsCollectionChanged;
            HandleDeletedConversation();
        }

        private void SelectedContactsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var activeItemExists = DetailsConductor.ActiveItem != null;
            var itemIsSelected = ContactListViewModel.SelectedContacts.Count == 1;
            if (itemIsSelected && !activeItemExists)
            {
                ShowDetails();
            }
            
            if (!itemIsSelected && activeItemExists)
            {
                HideDetails();
            }
        }

        private void ShowDetails()
        {
            DetailsConductor.ActivateItem(DetailsViewModelFactory.Create(ContactListViewModel.SelectedContacts));
        }

        private void HideDetails()
        {
            DetailsConductor.DeactivateItem(DetailsConductor.ActiveItem, true);
        }

        private void HandleDeletedConversation()
        {
            var conversationViewModel = DetailsConductor.ActiveItem as IConversationViewModel;
            if (conversationViewModel != null && conversationViewModel.State == ConversationViewModelStateEnum.Deleted)
            {
                ContactListViewModel.SelectedContacts.Clear();
                DetailsConductor.DeactivateItem(conversationViewModel, true);
            }
        }

        #region Explicit Interface Properties

        IScreen IMasterDetailsWorkspace.MasterScreen
        {
            get
            {
                return MasterScreen;
            }
        }

        #endregion
    }
}