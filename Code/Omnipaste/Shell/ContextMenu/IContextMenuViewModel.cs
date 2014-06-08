namespace Omnipaste.Shell.ContextMenu
{
    using Caliburn.Micro;

    public interface IContextMenuViewModel : IScreen
    {
        #region Public Properties

        bool IsSyncing { get; set; }

        IShellViewModel ShellViewModel { get; set; }

        #endregion

        #region Public Methods and Operators

        void ToggleSync();

        #endregion
    }
}