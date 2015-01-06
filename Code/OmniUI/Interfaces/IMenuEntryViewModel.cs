namespace OmniUI.Interfaces
{
    using System;

    public interface IMenuEntryViewModel : IDisposable
    {
        string Icon { get; }

        bool CanPerformAction { get; }

        void PerformAction();
    }
}
