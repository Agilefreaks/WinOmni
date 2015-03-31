namespace Omnipaste.Conversations.Menu
{
    using Omnipaste.Conversations;
    using OmniUI.Menu;

    public class NewMessageItemViewModel : WorkspaceItemViewModel<INewMessageWorkspace>
    {
        public override string Icon
        {
            get
            {
                return "appbar_new";
            }
        }
    }
}