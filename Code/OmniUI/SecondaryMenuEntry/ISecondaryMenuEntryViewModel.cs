namespace OmniUI.SecondaryMenuEntry
{
    using OmniUI.Intefaces;

    public interface ISecondaryMenuEntryViewModel : IMenuEntryViewModel
    {
        string ToolTipText { get; }
    }
}
