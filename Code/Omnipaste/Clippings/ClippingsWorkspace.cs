namespace Omnipaste.Clippings
{
    using Caliburn.Micro;
    using Omnipaste.Clippings.ClippingList;
    using Omnipaste.Properties;
    using OmniUI.Attributes;
    using OmniUI.Workspaces;

    [UseView(typeof(WorkspaceView))]
    public class ClippingsWorkspace : MasterDetailsWorkspace, IClippingsWorkspace
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
