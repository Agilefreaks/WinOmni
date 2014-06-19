namespace Omnipaste.Services
{
    using System;

    public interface ISessionManager
    {
        #region Public Events

        event EventHandler<EventArgs> SessionDestroyed;

        #endregion

        #region Public Methods and Operators

        void LogOut();

        #endregion
    }
}