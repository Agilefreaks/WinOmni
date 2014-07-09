namespace Omnipaste.Loading.UserToken
{
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.Framework;

    public class UserTokenViewModel : Screen, IUserTokenViewModel
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        private bool _isBusy;

        private string _message;

        private string _activationCode;

        #endregion

        #region Constructors and Destructors

        public UserTokenViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            ApplicationService = new ApplicationService();
        }

        #endregion

        #region Public Properties

        public string ActivationCode
        {
            get
            {
                return _activationCode;
            }
            set
            {
                _activationCode = value;
                NotifyOfPropertyChange(() => ActivationCode);
            }
        }

        public IApplicationService ApplicationService { get; set; }

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
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
                    NotifyOfPropertyChange(() => HasMessage);
                }
            }
        }

        public bool HasMessage
        {
            get
            {
                return !string.IsNullOrEmpty(Message);
            }
        }

        #endregion

        #region Public Methods and Operators

        protected override void OnActivate()
        {
            base.OnActivate();

            IsBusy = false;
            ActivationCode = string.Empty;
        }

        public void Authenticate()
        {
            Publish(new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Successful, ActivationCode));
        }

        public void Exit()
        {
            ApplicationService.ShutDown();
        }

        #endregion

        #region Methods

        private void Publish(TokenRequestResultMessage tokenRequestResultMessage)
        {
            _eventAggregator.PublishOnCurrentThread(tokenRequestResultMessage);
        }

        #endregion
    }
}