namespace OmniUI.Intefaces
{
    public interface ISecondaryMenuEntryViewModel : IHaveIcon, IHaveToolTipText
    {
        bool CanPerformAction { get; }

        void PerformAction();
    }
}
