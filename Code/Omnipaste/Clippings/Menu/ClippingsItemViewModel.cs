namespace Omnipaste.Clippings.Menu
{
    using OmniUI.Attributes;
    using OmniUI.Menu;
    using OmniUI.Menu.MainItem;

    [UseView(typeof(MainItemView))]
    public class ClippingsItemViewModel : WorkspaceItemViewModel<IClippingsWorkspace>
    {
        public override string Icon
        {
            get
            {
                return OmniUI.Resources.IconNames.SideMenuClippings;
            }
        }
    }
}
