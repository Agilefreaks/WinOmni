namespace Omnipaste.Services
{
    using System;

    public interface ISessionManager
    {
        #region Public Properties

        IObservable<EventArgs> SessionDestroyedObservable { get; }

        #endregion

        #region Public Methods and Operators

        void LogOut();

        #endregion
    }
}