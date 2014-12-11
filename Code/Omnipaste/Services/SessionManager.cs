namespace Omnipaste.Services
{
    using System;
    using System.Reactive.Subjects;
    using System.Net.Http;
    using Ninject;
    using OmniApi.Support;
    using OmniCommon.Interfaces;

    public class SessionManager : ISessionManager, IHttpResponseMessageHandler
    {
        #region Fields

        private readonly Subject<EventArgs> _sessionDestroyedObservable;

        #endregion

        #region Constructors and Destructors

        public SessionManager()
        {
            _sessionDestroyedObservable = new Subject<EventArgs>();
        }

        #endregion

        #region Public Properties

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        public IObservable<EventArgs> SessionDestroyedObservable
        {
            get
            {
                return _sessionDestroyedObservable;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void LogOut()
        {
            ConfigurationService.ResetAuthSettings();
            _sessionDestroyedObservable.OnNext(new EventArgs());
        }

        public void OnBadRequest()
        {
            LogOut();
        }

        #endregion
    }
}