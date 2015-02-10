namespace Omnipaste.Notification
{
    using System;
    using System.Windows.Threading;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.Interfaces;

    public abstract class NotificationViewModelBase : Screen, INotificationViewModel
    {
        #region Fields

        private readonly TimeSpan _defaultDimissInterval = TimeSpan.FromMilliseconds(700);

        private readonly TimeSpan _halfSecondInterval = TimeSpan.FromMilliseconds(500);

        private readonly TimeSpan _oneMinuteInterval = TimeSpan.FromMinutes(1);

        private DispatcherTimer _autoCloseTimer;

        private DispatcherTimer _deactivationTimer;

        private ViewModelStatusEnum _state = ViewModelStatusEnum.Closed;

        #endregion

        #region Public Properties

        [Inject]
        public IApplicationService ApplicationService { get; set; }

        public abstract object Identifier { get; }

        public abstract string Line1 { get; }

        public abstract string Line2 { get; }

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
                NotifyOfPropertyChange();
                if (State == ViewModelStatusEnum.Open)
                {
                    InitializeTimers();
                }
            }
        }

        public abstract string Title { get; }

        public abstract NotificationTypeEnum Type { get; }

        #endregion

        #region Public Methods and Operators

        public void Close()
        {
            State = ViewModelStatusEnum.Closed;
            _deactivationTimer.Start();
        }

        #endregion

        #region Methods

        public void Dismiss()
        {
            if (State == ViewModelStatusEnum.Closed)
            {
                return;
            }

            if (_autoCloseTimer.IsEnabled)
            {
                _autoCloseTimer.Stop();
            }

            _autoCloseTimer.Interval = _defaultDimissInterval;
            _autoCloseTimer.Start();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            State = ViewModelStatusEnum.Open;
        }

        private void Deactivate()
        {
            var conductor = ((IConductor)Parent);

            if (conductor != null)
            {
                conductor.DeactivateItem(this, true);
            }
        }

        private void InitializeAutoCloseTimer()
        {
            _autoCloseTimer = new DispatcherTimer(DispatcherPriority.Normal, ApplicationService.Dispatcher)
                                  {
                                      Interval = _oneMinuteInterval
                                  };

            _autoCloseTimer.Tick += (sender, arguments) =>
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
                                         Interval = _halfSecondInterval
                                     };
            _deactivationTimer.Tick += (sender, arguments) =>
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