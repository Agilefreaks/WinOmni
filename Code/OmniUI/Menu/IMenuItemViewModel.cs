namespace OmniUI.Menu
{
    using System;

    public interface IMenuItemViewModel : IDisposable
    {
        string Icon { get; }

        bool CanPerformAction { get; }

        void PerformAction();
    }
}
