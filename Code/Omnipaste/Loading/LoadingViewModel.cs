namespace Omnipaste.Loading
{
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.UserToken;

    public class LoadingViewModel : Screen, ILoadingViewModel
    {
        public IEventAggregator EventAggregator { get; set; }

        private LoadingViewModelStateEnum _state = LoadingViewModelStateEnum.Loading;

        [Inject]
        public IUserTokenViewModel UserTokenViewModel { get; set; }

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

        public LoadingViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            EventAggregator.Subscribe(this);
        }

        public void Handle(GetTokenFromUserMessage message)
        {
            State = LoadingViewModelStateEnum.AwaitingUserTokenInput;
        }

        public void Handle(ConfigurationCompletedMessage message)
        {
            TryClose();
        }

        public void Handle(TokenRequestResultMessage message)
        {
            State = LoadingViewModelStateEnum.Loading;
        }
    }
}