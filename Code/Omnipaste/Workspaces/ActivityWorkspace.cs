namespace Omnipaste.Workspaces
{
    using Omnipaste.ActivityList;
    using Omnipaste.Properties;
    using OmniUI.Attributes;
    using OmniUI.Workspace;

    [UseView(typeof(WorkspaceView))]
    public class ActivityWorkspace : MasterDetailsWorkspace, IActivityWorkspace
    {
        #region Constructors and Destructors

        public ActivityWorkspace(IActivityListViewModel activityListViewModel, IDetailsConductorViewModel detailsConductor)
            : base(activityListViewModel, detailsConductor)
        {
        }

        #endregion

        #region Public Properties

        public override string DisplayName
        {
            get
            {
                return Resources.Activity;
            }
        }
        
        #endregion
    }
}