namespace Omnipaste.Notification
{
    using Caliburn.Micro;

    public abstract class NotificationViewModelBase: Screen, INotificationViewModel
    {
        private ViewModelStatusEnum _state;

        #region Public Properties

        public string Message { get; set; }

        public abstract string Title { get; }

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

        protected override void OnActivate()
        {
            base.OnActivate();

            State = ViewModelStatusEnum.Open;
        }
        
        public virtual void Close()
        {
            State = ViewModelStatusEnum.Closed;
            //((IConductor)Parent).DeactivateItem(this, true);
        }

        #endregion
    }

    public enum ViewModelStatusEnum
    {
        Open,

        Closed
    }
}