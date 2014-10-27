namespace Omnipaste.Shell.Settings
{
    using Ninject;
    using OmniCommon.Interfaces;
    using OmniUI.Flyout;

    public interface ISettingsViewModel : IFlyoutViewModel
    {
        #region Public Properties

        ISessionManager SessionManager { get; set; }

        [Inject]
        IApplicationService ApplicationService { get; set; }

        #endregion

        #region Public Methods and Operators

        void Exit();

        void LogOut();

        #endregion
    }
}