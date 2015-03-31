namespace OmniUI.Framework
{
    using System;

    public interface ISubscriptionsManager
    {
        #region Public Methods and Operators

        void Add(IDisposable subscription);

        void ClearAll();

        #endregion
    }
}