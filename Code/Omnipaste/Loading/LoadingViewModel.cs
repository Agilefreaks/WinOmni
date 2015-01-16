namespace Omnipaste.Loading
{
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Loading.ActivationFailed;
    using Omnipaste.Loading.AndroidInstallGuide;
    using Omnipaste.Loading.Congratulations;
    using Omnipaste.Loading.ConnectionTroubleshooter;
    using Omnipaste.Loading.CreateClipping;
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
                    NotifyOfPropertyChange();
                }
            }
        }

        [Inject]
        public IUserTokenViewModel UserTokenViewModel { get; set; }

        [Inject]
        public IAndroidInstallGuideViewModel AndroidInstallGuideViewModel { get; set; }

        [Inject]
        public IActivationFailedViewModel ActivationFailedViewModel { get; set; }
        
        [Inject]
        public IConnectionTroubleshooterViewModel ConnectionTroubleshooterViewModel { get; set; }

        [Inject]
        public ICongratulationsViewModel CongratulationsViewModel { get; set; }

        [Inject]
        public ICreateClippingViewModel CreateClippingViewModel { get; set; }

        #endregion

        #region Public Methods and Operators

        public ILoadingViewModel Loading()
        {
            State = LoadingViewModelStateEnum.Loading;
            return this;
        }

        public void Handle(GetTokenFromUserMessage publishedEvent)
        {
            UserTokenViewModel.Message = publishedEvent.Message;
            ActiveItem = UserTokenViewModel;

            State = LoadingViewModelStateEnum.Other;
            EventAggregator.PublishOnCurrentThread(new ShowShellMessage());
        }

        public void Handle(TokenRequestResultMessage publishedEvent)
        {
            UserTokenViewModel.Deactivate(true);
            State = LoadingViewModelStateEnum.Loading;
        }

        public void Handle(ActivationFailedMessage activationFailedMessage)
        {
            ActivationFailedViewModel.Exception = activationFailedMessage.Exception;
            ActiveItem = ActivationFailedViewModel;
            State = LoadingViewModelStateEnum.Other;
            EventAggregator.PublishOnCurrentThread(new ShowShellMessage());
        }

        public void Handle(ShowAndroidInstallGuideMessage message)
        {
            AndroidInstallGuideViewModel.AndroidInstallLink = message.AndroidInstallLink;
            ActiveItem = AndroidInstallGuideViewModel;
            State = LoadingViewModelStateEnum.Other;
            EventAggregator.PublishOnCurrentThread(new ShowShellMessage());
        }

        public void Handle(AndroidInstallationCompleteMessage message)
        {
            AndroidInstallGuideViewModel.Deactivate(true);
            State = LoadingViewModelStateEnum.Loading;
        }

        public void Handle(ShowConnectionTroubleshooterMessage message)
        {
            ActiveItem = ConnectionTroubleshooterViewModel;
            State = LoadingViewModelStateEnum.Other;
            EventAggregator.PublishOnCurrentThread(new ShowShellMessage());
        }

        public void Handle(ShowCongratulationsMessage message)
        {
            ActiveItem = CongratulationsViewModel;
            State = LoadingViewModelStateEnum.Other;
        }

        public void Handle(ShowCreateClippingMessage message)
        {
            ActiveItem = CreateClippingViewModel;
            State = LoadingViewModelStateEnum.Other;
        }

        #endregion
    }
}