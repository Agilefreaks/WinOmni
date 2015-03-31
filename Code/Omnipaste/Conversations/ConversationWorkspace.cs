namespace Omnipaste.Conversations
{
    using Caliburn.Micro;
    using Omnipaste.Conversations.ContactList;
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

        #endregion

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