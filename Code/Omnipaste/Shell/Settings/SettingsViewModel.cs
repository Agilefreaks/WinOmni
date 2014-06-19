namespace Omnipaste.Shell.Settings
{
    using MahApps.Metro.Controls;
    using Ninject;
    using Omnipaste.Framework;
    using Omnipaste.Services;

    public class SettingsViewModel : FlyoutBaseViewModel, ISettingsViewModel
    {
        #region Constructors and Destructors

        public SettingsViewModel()
        {
            Position = Position.Right;
        }

        #endregion

        #region Public Properties

        [Inject]
        public ISessionManager SessionManager { get; set; }

        #endregion

        #region Public Methods and Operators

        public void LogOut()
        {
            SessionManager.LogOut();

            Close();
        }

        #endregion

        #region Methods

        private void Close()
        {
            IsOpen = false;
        }

        #endregion
    }
}