namespace Omnipaste.Workspaces
{
    using Omnipaste.ActivityList;
    using Omnipaste.Properties;
    using OmniUI.Attributes;

    [UseView("Omnipaste.Workspaces.WorkspaceView", IsFullyQualifiedName = true)]
    public class ActivityWorkspaceViewModel : Workspace, IActivityWorkspaceViewModel
    {
        #region Private Fields

        private const string ActivityIconName = "navigation_activity_icon";

        #endregion

        #region Constructors and Destructors

        public ActivityWorkspaceViewModel(IActivityListViewModel activityListViewModel)
            : base(activityListViewModel)
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

        public override string Icon
        {
            get
            {
                return ActivityIconName;
            }
        }

        #endregion
    }
}