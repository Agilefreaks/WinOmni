namespace Omnipaste.Workspaces
{
    using Omnipaste.MasterEventList;
    using Omnipaste.Properties;
    using OmniUI.Attributes;

    [UseView("Omnipaste.Workspaces.WorkspaceView", IsFullyQualifiedName = true)]
    public class EventsWorkspaceViewModel : Workspace, IEventsWorkspaceViewModel
    {
        #region Constructors and Destructors

        public EventsWorkspaceViewModel(IMasterEventListViewModel defaultScreen)
            : base(defaultScreen)
        {
        }

        #endregion

        #region Public Properties

        public override string DisplayName
        {
            get
            {
                return Resources.MasterEventListDisplayName;
            }
        }

        public override string Icon
        {
            get
            {
                return OmniUI.Resources.IconNames.SideMenuMessages;
            }
        }

        #endregion
    }
}