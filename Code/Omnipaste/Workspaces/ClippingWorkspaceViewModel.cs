namespace Omnipaste.Workspaces
{
    using Omnipaste.MasterClippingList;
    using Omnipaste.Properties;
    using OmniUI.Attributes;

    [UseView("Omnipaste.Workspaces.WorkspaceView", IsFullyQualifiedName = true)]
    public class ClippingWorkspaceViewModel : Workspace, IClippingWorkspaceViewModel
    {
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

        #endregion
    }
}