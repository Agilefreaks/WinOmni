namespace Omnipaste.Services
{
    using System;
    using Ninject;
    using Omni;
    using OmniCommon.Interfaces;

    public class SessionManager : ISessionManager
    {
        #region Public Events

        public event EventHandler<EventArgs> SessionDestroyed;

        #endregion

        #region Public Properties

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public IOmniService OmniService { get; set; }

        #endregion

        #region Public Methods and Operators

        public void LogOut()
        {
            OmniService.Stop();
            ConfigurationService.ResetAuthSettings();

            if (SessionDestroyed != null)
            {
                SessionDestroyed(this, new EventArgs());
            }
        }

        #endregion
    }
}