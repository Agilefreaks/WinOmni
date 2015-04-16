namespace Omnipaste.Conversations.Menu
{
    using Omnipaste.Conversations;
    using OmniUI.Attributes;
    using OmniUI.Menu;
    using OmniUI.Menu.MainItem;
    using OmniUI.Resources;

    [UseView(typeof(MainItemView))]
    public class PeopleItemViewModel : WorkspaceItemViewModel<IConversationWorkspace>
    {
        public override string Icon
        {
            get
            {
                return IconNames.SideMenuMessages;
            }
        }
    }
}