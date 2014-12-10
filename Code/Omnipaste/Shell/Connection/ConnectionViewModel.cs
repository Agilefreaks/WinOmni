namespace Omnipaste.Shell.Connection
{
    using System;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Omni;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;

    public class ConnectionViewModel : Screen, IConnectionViewModel
    {
        #region Fields

        private readonly IDisposable _statusObserver;

        private ConnectionStateEnum _state;

        #endregion

        #region Constructors and Destructors

        public ConnectionViewModel(IOmniService omniService)
        {
            _statusObserver =
                omniService.StatusChangedObservable.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(
                        newStatus =>
                        State =
                        newStatus == OmniServiceStatusEnum.Started
                            ? ConnectionStateEnum.Connected
                            : ConnectionStateEnum.Disconnected);
        }

        #endregion

        #region Public Properties
        
        public ConnectionStateEnum State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            _statusObserver.Dispose();
        }

        #endregion
    }
}