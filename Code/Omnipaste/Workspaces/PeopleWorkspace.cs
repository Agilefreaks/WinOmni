namespace Omnipaste.Workspaces
{
    using Caliburn.Micro;
    using Omnipaste.ContactList;
    using Omnipaste.Properties;
    using OmniUI.Attributes;
    using OmniUI.Workspace;

    [UseView(typeof(WorkspaceView))]
    public class PeopleWorkspace : MasterDetailsWorkspace, IPeopleWorkspace
    {
        #region Constructors and Destructors

        public PeopleWorkspace(IContactListViewModel masterScreen, IDetailsConductorViewModel detailsConductor)
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