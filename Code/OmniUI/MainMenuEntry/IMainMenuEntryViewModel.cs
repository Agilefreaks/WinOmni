namespace OmniUI.MainMenuEntry
{
    using OmniUI.Interfaces;

    public interface IMainMenuEntryViewModel : IMenuEntryViewModel
    {
        string DisplayName { get; }

        bool IsOpen { get; }
    }
}
