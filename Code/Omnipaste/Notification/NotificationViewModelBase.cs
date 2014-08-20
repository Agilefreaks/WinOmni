namespace Omnipaste.Notification
{
    using System;
    using System.Windows;
    using System.Windows.Threading;
    using Caliburn.Micro;

    public abstract class NotificationViewModelBase : Screen, INotificationViewModel
    {
        #region Fields

        private readonly TimeSpan _deactivationDuration = new TimeSpan(0, 0, 0, 0, 500);

        private readonly TimeSpan _timeoutPeriod = new TimeSpan(0, 0, 6);

        private DispatcherTimer _autoCloseTimer;

        private DispatcherTimer _deactivationTimer;

        private ViewModelStatusEnum _state;

        private DispatcherTimer _dispatcherTimer;

        #endregion

        #region Public Properties

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

        protected void DelayedClose(int milliseconds = 500)
        {
            if (_autoCloseTimer.IsEnabled)
            {
                _autoCloseTimer.Stop();
            }

            _dispatcherTimer = new DispatcherTimer(
                new TimeSpan(0, 0, 0, 0, milliseconds),
                DispatcherPriority.Normal,
                (sender, args) =>
                {
                    _dispatcherTimer.Stop();
                    Close();
                },
                Application.Current.Dispatcher);
        }

        public void Close()
        {
            State = ViewModelStatusEnum.Closed;
            
            _deactivationTimer = new DispatcherTimer(
                _deactivationDuration,
                DispatcherPriority.Normal,
                (sender, args) =>
                {
                    _deactivationTimer.Stop();
                    Deactivate();
                },
                Dispatcher.CurrentDispatcher);
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();

            _autoCloseTimer = new DispatcherTimer(
                _timeoutPeriod,
                DispatcherPriority.Normal,
                (sender, args) =>
                {
                    _autoCloseTimer.Stop();
                    Close();
                },
                Dispatcher.CurrentDispatcher);

            State = ViewModelStatusEnum.Open;
        }

        private void Deactivate()
        {
            ((IConductor)Parent).DeactivateItem(this, true);
        }

        #endregion
    }
}