namespace OmniUI.Menu.MainItem
{
    public interface IMainItemViewModel : IMenuItemViewModel
    {
        string DisplayName { get; }

        bool IsOpen { get; }
    }
}
