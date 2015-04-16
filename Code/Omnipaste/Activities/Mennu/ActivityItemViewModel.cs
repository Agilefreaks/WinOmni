namespace Omnipaste.Activities.Mennu
{
    using OmniUI.Attributes;
    using OmniUI.Menu;
    using OmniUI.Menu.MainItem;
    using OmniUI.Resources;

    [UseView(typeof(MainItemView))]
    public class ActivityItemViewModel : WorkspaceItemViewModel<IActivityWorkspace>
    {
        public override string Icon
        {
            get
            {
                return IconNames.SideMenuActivity;
            }
        }
    }
}
