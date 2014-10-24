namespace OmniUI.HeaderButton
{
    public interface IHeaderButtonViewModel
    {
        string ButtonToolTip { get; }

        string Icon { get; }

        void PerformAction();
    }
}