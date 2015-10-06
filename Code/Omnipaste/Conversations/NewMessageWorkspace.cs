namespace Omnipaste.Conversations
{
    using Ninject;
    using Omnipaste.Conversations.ContactList;
    using Omnipaste.Conversations.Conversation;
    using Omnipaste.Framework;
    using Omnipaste.Properties;
    using OmniUI.Attributes;
    using OmniUI.Workspaces;

    [UseView(typeof(WorkspaceView))]
    public class NewMessageWorkspace : MasterDetailsWorkspace, INewMessageWorkspace
    {
        public override string DisplayName
        {
            get
            {
                return Resources.GroupMessage;
            }
        }

        public new IContactListViewModel MasterScreen { get; private set; }

        [Inject]
        public IDetailsViewModelFactory DetailsViewModelFactory { get; set; }


        public NewMessageWorkspace(IContactListViewModel masterScreen, IDetailsConductorViewModel detailsConductor)
            : base(masterScreen, detailsConductor)
        {
            masterScreen.CanSelectMultipleItems = true;
            MasterScreen = masterScreen;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            if (DetailsConductor.ActiveItem == null)
            {
                var detailsViewModelWithHeader = DetailsViewModelFactory.Create(MasterScreen.SelectedContacts);
                ((IConversationHeaderViewModel)detailsViewModelWithHeader.HeaderViewModel).State = ConversationHeaderStateEnum.Edit;
                DetailsConductor.ActivateItem(detailsViewModelWithHeader);
            }
        }
    }
}