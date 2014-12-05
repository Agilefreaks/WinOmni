namespace Omnipaste.Services
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using OmniCommon.Helpers;

    public class UiRefreshService : IUiRefreshService
    {
        #region Fields

        private readonly Subject<Unit> _refreshSubject;

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
            _disposable =
                Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(5))
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .Subscribe(_ => _refreshSubject.OnNext(new Unit()));
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