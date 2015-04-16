namespace OmniUI.Framework.Services
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;

    public class UiRefreshService : IUiRefreshService
    {
        #region Fields

        private readonly Subject<Unit> _refreshSubject;

        private readonly TimeSpan _uiRefreshInterval = TimeSpan.FromSeconds(5);

        private IDisposable _disposable;

        #endregion

        #region Constructors and Destructors

        public UiRefreshService()
        {
            _refreshSubject = new Subject<Unit>();
        }

        #endregion

        #region Public Properties

        public IObservable<Unit> RefreshObservable
        {
            get
            {
                return _refreshSubject;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Start()
        {
            Stop();
            _disposable =
                Observable.Timer(TimeSpan.Zero, _uiRefreshInterval)
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(_ => _refreshSubject.OnNext(new Unit()));
        }

        public void Stop()
        {
            if (_disposable == null)
            {
                return;
            }

            _disposable.Dispose();
            _disposable = null;
        }

        #endregion
    }
}