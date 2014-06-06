namespace Omnipaste.UserToken
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

        #endregion

        #region Constructors and Destructors

        public UserTokenViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            ApplicationWrapper = new ApplicationWrapper();
        }

        #endregion

        #region Public Properties

        public string ActivationCode { get; set; }

        public IApplicationWrapper ApplicationWrapper { get; set; }

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
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        protected override void OnActivate()
        {
            base.OnActivate();

            IsBusy = false;
        }

        public void Authenticate()
        {
            Publish(new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Successful, ActivationCode));
        }

        public void Exit()
        {
            ApplicationWrapper.ShutDown();
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