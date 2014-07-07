namespace Omnipaste.Shell.ContextMenu
{
    using Caliburn.Micro;
    using CustomizedClickOnce.Common;

    public interface IContextMenuViewModel : IScreen
    {
        #region Public Properties

        bool AutoStart { get; set; }
        
        IClickOnceHelper ClickOnceHelper { get; set; }

        bool IsStopped { get; set; }

        #endregion

        #region Public Methods and Operators

        void Show();

        void ShowBaloon(string baloonTitle, string baloonMessage);

        void ToggleAutoStart();

        void ToggleSync();

        #endregion
    }
}