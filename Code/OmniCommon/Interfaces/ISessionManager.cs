namespace OmniCommon.Interfaces
{
    using System;

    public interface ISessionManager
    {
        #region Public Methods and Operators

        void LogOut();

        IObservable<EventArgs> SessionDestroyedObservable();

        #endregion
    }
}