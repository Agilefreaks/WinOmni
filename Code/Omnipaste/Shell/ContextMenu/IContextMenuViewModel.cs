namespace Omnipaste.Shell.ContextMenu
{
    using Caliburn.Micro;

    public interface IContextMenuViewModel : IScreen
    {
        #region Public Properties

        bool IsStopped { get; set; }

        #endregion

        #region Public Methods and Operators

        void Show();

        void ToggleSync();

        void ToggleAutoStart();

        #endregion
    }
}