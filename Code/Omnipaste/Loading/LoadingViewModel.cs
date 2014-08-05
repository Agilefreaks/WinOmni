namespace Omnipaste.Loading
{
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.Loading.ActivationFailed;
    using Omnipaste.Loading.AndroidInstallGuide;
    using Omnipaste.Loading.UserToken;

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

        [Inject]
        public IAndroidInstallGuideViewModel AndroidInstallGuideViewModel { get; set; }

        public ILoadingViewModel Loading()
        {
            State = LoadingViewModelStateEnum.Loading;
            return this;
        }

        [Inject]
        public IActivationFailedViewModel ActivationFailedViewModel { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Handle(GetTokenFromUserMessage publishedEvent)
        {
            UserTokenViewModel.Message = publishedEvent.Message;
            ActiveItem = UserTokenViewModel;

            State = LoadingViewModelStateEnum.Other;
        }

        public void Handle(TokenRequestResultMessage publishedEvent)
        {
            UserTokenViewModel.Deactivate(true);
            State = LoadingViewModelStateEnum.Loading;
        }

        public void Handle(ActivationFailedMessage activationFailedMessage)
        {
            ActiveItem = ActivationFailedViewModel;
            State = LoadingViewModelStateEnum.Other;
        }

        public void Handle(ShowAndroidInstallGuideMessage message)
        {
            ActiveItem = AndroidInstallGuideViewModel;
            State = LoadingViewModelStateEnum.Other;
        }

        public void Handle(AndroidInstallationCompleteMessage message)
        {
            AndroidInstallGuideViewModel.Deactivate(true);
            State = LoadingViewModelStateEnum.Loading;
        }

        #endregion
    }
}