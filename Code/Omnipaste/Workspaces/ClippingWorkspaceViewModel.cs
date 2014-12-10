namespace Omnipaste.Workspaces
{
    using Omnipaste.MasterClippingList;
    using Omnipaste.Properties;
    using OmniUI.Attributes;

    [UseView("Omnipaste.Workspaces.WorkspaceView", IsFullyQualifiedName = true)]
    public class ClippingWorkspaceViewModel : Workspace, IClippingWorkspaceViewModel
    {
        #region Private Fields

        private const string ClippingsIconName = "navigation_clippings_icon";

        #endregion

        #region Constructors and Destructors

        public ClippingWorkspaceViewModel(IMasterClippingListViewModel defaultScreen)
            : base(defaultScreen)
        {
        }

        #endregion

        #region Public Properties

        public override string DisplayName
        {
            get
            {
                return Resources.MasterClippingListDisplayName;
            }
        }

        public override string Icon
        {
            get
            {
                return ClippingsIconName;
            }
        }

        #endregion
    }
}