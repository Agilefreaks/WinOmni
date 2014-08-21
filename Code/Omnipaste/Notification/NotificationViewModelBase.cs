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

        private ViewModelStatusEnum _state = ViewModelStatusEnum.Closed;

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

        protected void Dismiss(int delay = 1000)
        {
            if (State == ViewModelStatusEnum.Closed)
            {
                return;
            }

            if (_autoCloseTimer.IsEnabled)
            {
                _autoCloseTimer.Stop();
            }

            _autoCloseTimer.Interval = new TimeSpan(0, 0, 0, 0, delay);
            _autoCloseTimer.Start();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            State = ViewModelStatusEnum.Open;
        }

        private void Deactivate()
        {
            ((IConductor)Parent).DeactivateItem(this, true);
        }

        private void InitializeTimers()
        {
            InitializeAutoCloseTimer();
            InitializeDeactivationTimer();
        }

        private void InitializeAutoCloseTimer()
        {
            _autoCloseTimer = new DispatcherTimer(DispatcherPriority.Normal, ApplicationService.Dispatcher)
                              {
                                  Interval = _oneMinuteInterval
                              };

            _autoCloseTimer.Tick += (sender, args) =>
            {
                _autoCloseTimer.Stop();
                Close();
            };

            _autoCloseTimer.Start();
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

        public override void NotifyOfPropertyChange(string propertyName)
        {
            base.NotifyOfPropertyChange(propertyName);

            if (propertyName == "State")
            {
                if (State == ViewModelStatusEnum.Open)
                {
                    InitializeTimers();
                }
            }
        }

        #endregion
    }
}