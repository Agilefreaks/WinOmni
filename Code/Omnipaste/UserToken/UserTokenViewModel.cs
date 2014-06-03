namespace Omnipaste.UserToken
{
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;

    public class UserTokenViewModel : Screen, IUserTokenViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private string _message;
        
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
        }

        public void Register()
        {
            Publish(new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Successful, ActivationCode));
        }

        private void Publish(TokenRequestResultMessage tokenRequestResultMessage)
        {
            _eventAggregator.PublishOnCurrentThread(tokenRequestResultMessage);
        }
    }
}