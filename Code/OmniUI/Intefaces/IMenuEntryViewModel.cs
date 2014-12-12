namespace OmniUI.Intefaces
{
    using System;

    public interface IMenuEntryViewModel : IDisposable
    {
        string Icon { get; }

        bool CanPerformAction { get; }

        void PerformAction();
    }
}
