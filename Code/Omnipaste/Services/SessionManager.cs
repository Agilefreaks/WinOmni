namespace Omnipaste.Services
{
    using System;
    using System.Reactive.Subjects;
    using Ninject;
    using OmniApi.Resources.v1;
    using OmniApi.Support;
    using OmniCommon.ExtensionMethods;
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

        [Inject]
        public IDevices Devices { get; set; }

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
            Devices.Remove(ConfigurationService.DeviceId).RunToCompletionSynchronous();
            ConfigurationService.ClearSettings();
            _sessionDestroyedObservable.OnNext(new EventArgs());
        }

        public void OnBadRequest()
        {
            LogOut();
        }

        #endregion
    }
}