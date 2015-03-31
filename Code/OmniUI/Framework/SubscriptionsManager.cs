namespace OmniUI.Framework
{
    using System;
    using System.Reactive.Disposables;

    public class SubscriptionsManager : ISubscriptionsManager
    {
        #region Fields

        private CompositeDisposable _subscriptions;

        #endregion

        #region Constructors and Destructors

        public SubscriptionsManager()
        {
            Init();
        }

        #endregion

        #region Properties

        protected bool IsInitialized
        {
            get
            {
                return _subscriptions != null;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Add(IDisposable subscription)
        {
            if (!IsInitialized)
            {
                Init();
            }

            _subscriptions.Add(subscription);
        }

        public void ClearAll()
        {
            if (_subscriptions == null)
            {
                return;
            }

            _subscriptions.Dispose();
            _subscriptions = null;
        }

        #endregion

        #region Methods

        protected void Init()
        {
            if (IsInitialized)
            {
                ClearAll();
            }
            else
            {
                _subscriptions = new CompositeDisposable();
            }
        }

        #endregion
    }
}