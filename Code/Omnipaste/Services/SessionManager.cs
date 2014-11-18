namespace Omnipaste.Services
{
    using System;
    using System.Reactive.Subjects;
    using Ninject;
    using Omni;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Interfaces;

    public class SessionManager : ISessionManager
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

        [Inject]
        public IOmniService OmniService { get; set; }

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
            OmniService.Stop();
            ConfigurationService.ResetAuthSettings();
            _sessionDestroyedObservable.OnNext(new EventArgs());
        }

        #endregion
    }
}