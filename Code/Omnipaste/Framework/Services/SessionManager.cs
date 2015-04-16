namespace Omnipaste.Framework.Services
{
    using System;
    using System.Collections.Generic;
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

        private readonly Subject<SessionItemChangeEventArgs> _itemChangedObservable;

        private readonly Dictionary<string, object> _data;

        #endregion

        #region Constructors and Destructors

        public SessionManager()
        {
            _sessionDestroyedObservable = new Subject<EventArgs>();
            _itemChangedObservable = new Subject<SessionItemChangeEventArgs>();
            _data = new Dictionary<string, object>();
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

        public IObservable<SessionItemChangeEventArgs> ItemChangedObservable
        {
            get
            {
                return _itemChangedObservable;
            }
        }

        public object this[string key]
        {
            get
            {
                return _data.ContainsKey(key) ? _data[key] : null;
            }

            set
            {
                var changedArgs = new SessionItemChangeEventArgs(key, value, this[key]);
                _data[key] = value;
                _itemChangedObservable.OnNext(changedArgs);
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