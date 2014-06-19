namespace Omnipaste.Loading
{
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.UserToken;

    public class LoadingViewModel : Conductor<IScreen>.Collection.OneActive, ILoadingViewModel
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
            ActiveItem = UserTokenViewModel;

            State = LoadingViewModelStateEnum.AwaitingUserTokenInput;
        }

        public void Handle(TokenRequestResultMessage publishedEvent)
        {
            UserTokenViewModel.Deactivate(true);
            State = LoadingViewModelStateEnum.Loading;
        }

        #endregion
    }
}