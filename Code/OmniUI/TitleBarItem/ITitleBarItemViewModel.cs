namespace OmniUI.TitleBarItem
{
    using OmniUI.Interfaces;

    public interface ITitleBarItemViewModel : IMenuEntryViewModel
    {
        string Tag { get; }
    }
}
