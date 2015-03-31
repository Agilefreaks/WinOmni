namespace Omnipaste.Workspaces.Clippings
{
    using Caliburn.Micro;
    using Omnipaste.ClippingList;
    using Omnipaste.Properties;
    using OmniUI.Attributes;
    using OmniUI.Workspace;

    [UseView(typeof(WorkspaceView))]
    public class ClippingsWorkspace : MasterDetailsWorkspace, IClippingWorkspace
    {
        #region Constructors and Destructors

        public ClippingsWorkspace(IClippingListViewModel masterScreen, IDetailsConductorViewModel detailsConductor)
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
                return Resources.Clippings;
            }
        }

        public new IClippingListViewModel MasterScreen { get; private set; }

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
