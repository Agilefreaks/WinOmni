namespace Omnipaste.Framework.Services.Monitors.SettingsMonitor
{
    using System;
    using System.Reactive.Subjects;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Interfaces;

    public abstract class SettingsMonitorBase<TSetting> : ISettingsMonitor<TSetting>
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly string _settingName;

        private readonly Subject<TSetting> _subject;

        private IDisposable _settingChangeSubscription;

        #endregion

        #region Constructors and Destructors

        protected SettingsMonitorBase(IConfigurationService configurationService, string settingName)
        {
            _configurationService = configurationService;
            _settingName = settingName;
            _subject = new Subject<TSetting>();
        }

        #endregion

        #region Public Properties

        public IObservable<TSetting> SettingObservable
        {
            get
            {
                return _subject;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Start()
        {
            Stop();
            _settingChangeSubscription =
                _configurationService.SettingsChangedObservable.SubscribeToSettingChange<TSetting>(
                    _settingName,
                    _subject.OnNext);
        }

        public void Stop()
        {
            if (_settingChangeSubscription == null)
            {
                return;
            }

            _settingChangeSubscription.Dispose();
            _settingChangeSubscription = null;
        }

        #endregion
    }
}