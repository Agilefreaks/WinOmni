namespace Omnipaste.Shell.Settings
{
    using MahApps.Metro.Controls;
    using Ninject;
    using OmniCommon.Interfaces;
    using Omnipaste.Framework;

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

        [Inject]
        public IApplicationService ApplicationService { get; set; }

        #endregion

        #region Public Methods and Operators

        public void LogOut()
        {
            SessionManager.LogOut();

            Close();
        }

        public void Exit()
        {
            ApplicationService.ShutDown();
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