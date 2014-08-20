namespace Omnipaste.Notification
{
    using System;
    using System.Windows.Threading;
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.Framework;

    public abstract class NotificationViewModelBase : Screen, INotificationViewModel
    {
        #region Fields

        private readonly TimeSpan _halfSecondInterval = new TimeSpan(0, 0, 0, 0, 500);

        private readonly TimeSpan _oneMinuteInterval = new TimeSpan(0, 0, 60);

        private DispatcherTimer _autoCloseTimer;

        private DispatcherTimer _deactivationTimer;

        private ViewModelStatusEnum _state;

        #endregion

        #region Public Properties

        [Inject]
        public IApplicationService ApplicationService { get; set; }

        public string Message { get; set; }

        public ViewModelStatusEnum State
        {
            get
            {
                return _state;
            }
            set
            {
                if (value == _state)
                {
                    return;
                }
                _state = value;
                NotifyOfPropertyChange(() => State);
            }
        }

        public abstract string Title { get; }

        #endregion

        #region Public Methods and Operators

        public void Close()
        {
            State = ViewModelStatusEnum.Closed;

            _deactivationTimer.Start();
        }

        #endregion

        #region Methods

        protected void DelayedClose(int milliseconds = 500)
        {
            if (_autoCloseTimer.IsEnabled)
            {
                _autoCloseTimer.Stop();
            }

            _autoCloseTimer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds);
            _autoCloseTimer.Start();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            InitializeTimers();

            _autoCloseTimer.Start();
            State = ViewModelStatusEnum.Open;
        }

        private void Deactivate()
        {
            ((IConductor)Parent).DeactivateItem(this, true);
        }

        private void InitializeAutoCloseTimer()
        {
            _autoCloseTimer = new DispatcherTimer(DispatcherPriority.Normal, ApplicationService.Dispatcher)
                              {
                                  Interval
                                      =
                                      _oneMinuteInterval
                              };
            _autoCloseTimer.Tick += (sender, args) =>
            {
                _autoCloseTimer.Stop();
                Close();
            };
        }

        private void InitializeDeactivationTimer()
        {
            _deactivationTimer = new DispatcherTimer(DispatcherPriority.Normal, ApplicationService.Dispatcher)
                                 {
                                     Interval
                                         =
                                         _halfSecondInterval
                                 };
            _deactivationTimer.Tick += (sender, args) =>
            {
                _deactivationTimer.Stop();
                Deactivate();
            };
        }

        private void InitializeTimers()
        {
            InitializeAutoCloseTimer();
            InitializeDeactivationTimer();
        }

        #endregion
    }
}