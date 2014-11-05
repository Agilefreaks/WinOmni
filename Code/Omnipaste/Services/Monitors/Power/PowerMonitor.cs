namespace Omnipaste.Services.Monitors.Power
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Microsoft.Win32;
    using OmniCommon.Helpers;
    using Omnipaste.ExtensionMethods;

    public class PowerMonitor : IPowerMonitor
    {
        #region Fields

        private readonly ReplaySubject<PowerModes> _powerModesSubject;

        private readonly ISystemPowerHelper _systemPowerHelper;

        private IDisposable _eventsThreadObserver;

        private IDisposable _powerModeChangedObserver;

        #endregion

        #region Constructors and Destructors

        public PowerMonitor(ISystemPowerHelper systemPowerHelper)
        {
            _systemPowerHelper = systemPowerHelper;
            _powerModesSubject = new ReplaySubject<PowerModes>(0);
        }

        #endregion

        #region Public Properties

        public IObservable<PowerModes> PowerModesObservable
        {
            get
            {
                return _powerModesSubject;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Start()
        {
            DisposeObservers();
            _powerModeChangedObserver =
                _systemPowerHelper.PowerModeChangedObservable.Select(eventArgs => eventArgs.Mode)
                    .Where(mode => mode == PowerModes.Resume || mode == PowerModes.Suspend)
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .Subscribe(_powerModesSubject);
            _eventsThreadObserver =
                _systemPowerHelper.EventsThreadShutdownObservable.SubscribeOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(_ => DisposeObservers());
        }

        public void Stop()
        {
            DisposeObservers();
        }

        #endregion

        #region Methods

        private void DisposeEventsThreadObserver()
        {
            if (_eventsThreadObserver != null)
            {
                _eventsThreadObserver.Dispose();
            }
        }

        private void DisposeObservers()
        {
            DisposeEventsThreadObserver();
            DisposePowerModeChangedObserver();
        }

        private void DisposePowerModeChangedObserver()
        {
            if (_powerModeChangedObserver != null)
            {
                _powerModeChangedObserver.Dispose();
            }
        }

        #endregion
    }
}