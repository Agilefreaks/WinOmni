namespace Omnipaste.Loading
{
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.EventAggregatorMessages;

    public class LoadingViewModel : Screen, ILoadingViewModel
    {
        private LoadingViewModelStateEnum _state;

        public LoadingViewModelStateEnum State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                NotifyOfPropertyChange(() => State);
            }
        }

        public void Handle(GetTokenFromUserMessage message)
        {
            State = LoadingViewModelStateEnum.AwaitingUserTokenInput;
        }

        public void Handle(ConfigurationCompletedMessage message)
        {
            TryClose();
        }
    }
}