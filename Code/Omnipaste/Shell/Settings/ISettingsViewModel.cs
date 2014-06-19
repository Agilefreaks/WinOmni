namespace Omnipaste.Shell.Settings
{
    using Omnipaste.Framework;
    using Omnipaste.Services;

    public interface ISettingsViewModel : IFlyoutViewModel
    {
        #region Public Properties

        ISessionManager SessionManager { get; set; }

        #endregion

        #region Public Methods and Operators

        void LogOut();

        #endregion
    }
}