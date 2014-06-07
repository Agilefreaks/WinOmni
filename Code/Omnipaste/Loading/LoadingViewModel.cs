namespace Omnipaste.Loading
{
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.UserToken;

    public class LoadingViewModel : Screen, ILoadingViewModel
    {
        #region Fields

        private LoadingViewModelStateEnum _state = LoadingViewModelStateEnum.Loading;

        #endregion

        #region Constructors and Destructors

        public LoadingViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            EventAggregator.Subscribe(this);
        }

        #endregion

        #region Public Properties

        public IEventAggregator EventAggregator { get; set; }

        public LoadingViewModelStateEnum State
        {
            get
            {
                return _state;
            }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    NotifyOfPropertyChange(() => State);
                }
            }
        }

        [Inject]
        public IUserTokenViewModel UserTokenViewModel { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Handle(GetTokenFromUserMessage publishedEvent)
        {
            UserTokenViewModel.Message = publishedEvent.Message;
            State = LoadingViewModelStateEnum.AwaitingUserTokenInput;
        }

        public void Handle(ConfigurationCompletedMessage publishedEvent)
        {
            TryClose();
        }

        public void Handle(TokenRequestResultMessage publishedEvent)
        {
            State = LoadingViewModelStateEnum.Loading;
        }

        #endregion
    }
}