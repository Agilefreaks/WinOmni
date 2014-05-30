using System.Windows.Navigation;
using MahApps.Metro.Controls;
using Omnipaste.Framework;

namespace Omnipaste.UserToken
{
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;

    public class UserTokenViewModel : FlyoutBaseViewModel, IUserTokenViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private bool _isBusy;

        private string _message;

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    NotifyOfPropertyChange(() => IsBusy);
                }
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    NotifyOfPropertyChange(() => Message);
                }
            }
        }

        public string ActivationCode { get; set; }

        public UserTokenViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            IsModal = true;
            Position = Position.Right;
        }

        public void Register()
        {
            IsBusy = true;
            Publish(new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Successful, ActivationCode));
        }

        private void Publish(TokenRequestResultMessage tokenRequestResultMessage)
        {
            _eventAggregator.PublishOnBackgroundThread(tokenRequestResultMessage);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            IsOpen = true;
            IsBusy = false;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            
            IsOpen = false;
        }

        public override void NotifyOfPropertyChange(string propertyName)
        {
            base.NotifyOfPropertyChange(propertyName);
            if (propertyName == "IsOpen" && !IsOpen)
            {
                ((IDeactivate)this).Deactivate(true);
            }
        }
    }
}