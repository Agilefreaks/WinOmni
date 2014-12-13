namespace OmniUI.MainMenuEntry
{
    using OmniUI.Intefaces;

    public interface IMainMenuEntryViewModel : IMenuEntryViewModel
    {
        string DisplayName { get; }

        bool IsOpen { get; }
    }
}
