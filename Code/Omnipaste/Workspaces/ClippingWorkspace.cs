namespace Omnipaste.Workspaces
{
    using Omnipaste.MasterClippingList;
    using Omnipaste.Properties;
    using OmniUI.Attributes;

    [UseView(typeof(SingleItemWorkspaceView))]
    public class ClippingWorkspace : SingleItemWorkspace, IClippingWorkspace
    {
        #region Constructors and Destructors

        public ClippingWorkspace(IMasterClippingListViewModel defaultScreen)
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