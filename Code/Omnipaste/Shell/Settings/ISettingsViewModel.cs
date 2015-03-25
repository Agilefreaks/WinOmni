namespace Omnipaste.Shell.Settings
{
    using Ninject;
    using OmniCommon.Interfaces;
    using OmniUI.Flyout;
    using Omnipaste.Services;

    public interface ISettingsViewModel : IFlyoutViewModel
    {
        #region Public Properties

        ISessionManager SessionManager { get; set; }

        [Inject]
        IApplicationService ApplicationService { get; set; }

        bool IsSMSSuffixEnabled { get; set; }

        #endregion

        #region Public Methods and Operators

        void Exit();

        void LogOut();

        void RefreshContacts();

        #endregion
    }
}