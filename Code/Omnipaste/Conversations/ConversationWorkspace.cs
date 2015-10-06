namespace Omnipaste.Conversations
{
    using System.Collections.Specialized;
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.Conversations.ContactList;
    using Omnipaste.Properties;
    using Omnipaste.WorkspaceDetails;
    using OmniUI.Attributes;
    using OmniUI.Workspaces;

    [UseView(typeof(WorkspaceView))]
    public class ConversationWorkspace : MasterDetailsWorkspace, IConversationWorkspace
    {
        #region Constructors and Destructors

        public ConversationWorkspace(IContactListViewModel masterScreen, IDetailsConductorViewModel detailsConductor)
            : base(masterScreen, detailsConductor)
        {
            MasterScreen = masterScreen;
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

        public new IContactListViewModel MasterScreen { get; private set; }

        [Inject]
        public IDetailsViewModelFactory DetailsViewModelFactory { get; set; }

        #endregion

        protected override void OnActivate()
        {
            base.OnActivate();
            MasterScreen.SelectedContacts.CollectionChanged += SelectedContactsCollectionChanged;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            MasterScreen.SelectedContacts.CollectionChanged -= SelectedContactsCollectionChanged;
        }

        private void SelectedContactsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var activeItemExists = DetailsConductor.ActiveItem != null;
            var itemIsSelected = MasterScreen.SelectedContacts.Count == 1;
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
            DetailsConductor.ActivateItem(DetailsViewModelFactory.Create(MasterScreen.SelectedContacts));
        }

        private void HideDetails()
        {
            DetailsConductor.DeactivateItem(DetailsConductor.ActiveItem, true);
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