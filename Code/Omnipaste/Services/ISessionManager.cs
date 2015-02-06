namespace Omnipaste.Services
{
    using System;

    public interface ISessionManager
    {
        #region Public Properties

        IObservable<EventArgs> SessionDestroyedObservable { get; }

        IObservable<SessionItemChangeEventArgs> ItemChangedObservable { get; }

        object this[string key] { get; set; }

        #endregion

        #region Public Methods and Operators

        void LogOut();

        #endregion
    }
}