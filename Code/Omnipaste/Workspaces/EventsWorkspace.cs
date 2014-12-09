namespace Omnipaste.Workspaces
{
    using Omnipaste.MasterEventList;
    using Omnipaste.Properties;
    using OmniUI.Attributes;

    [UseView("Omnipaste.Workspaces.SingleItemWorkspaceView", IsFullyQualifiedName = true)]
    public class EventsWorkspace : SingleItemWorkspace, IEventsWorkspace
    {
        #region Constructors and Destructors

        public EventsWorkspace(IMasterEventListViewModel defaultScreen)
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