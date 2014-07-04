namespace Omnipaste.Shell.Settings
{
    using OmniCommon.Interfaces;
    using Omnipaste.Framework;

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