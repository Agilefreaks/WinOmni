namespace OmniUI.SecondaryMenuEntry
{
    using OmniUI.Interfaces;

    public interface ISecondaryMenuEntryViewModel : IMenuEntryViewModel
    {
        string ToolTipText { get; }
    }
}
