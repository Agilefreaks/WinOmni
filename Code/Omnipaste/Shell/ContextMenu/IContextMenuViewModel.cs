namespace Omnipaste.Shell.ContextMenu
{
    using Caliburn.Micro;

    public interface IContextMenuViewModel : IScreen
    {
        #region Public Properties

        bool AutoStart { get; set; }
        
        bool IsStopped { get; set; }

        string IconSource { get; set; }

        string TooltipText { get; }

        #endregion

        #region Public Methods and Operators

        void Show();

        void ShowBalloon(string balloonTitle, string balloonMessage);

        void ToggleSync();

        #endregion
    }
}