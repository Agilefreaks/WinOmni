namespace OmnipasteWPF.ViewModels.MainView
{
    using System;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Cinch;
    using OmniCommon.EventAggregatorMessages;
    using OmniCommon.Interfaces;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;
    using OmnipasteWPF.DataProviders;
    using OmnipasteWPF.ViewModels.GetTokenFromUser;
    using OmnipasteWPF.ViewModels.TrayIcon;

    public class MainViewModel : ViewModel, IHandle<GetTokenFromUserMessage>
    {
        private IEventAggregator _eventAggregator;

        private IGetTokenFromUserViewModel _getTokenFromUserViewModel;

        private ITrayIconViewModel _trayIconViewModel;

        public ITrayIconViewModel TrayIconViewModel
        {
            get
            {
                return _trayIconViewModel ?? (_trayIconViewModel = new TrayIconViewModel());
            }

            set
            {
                _trayIconViewModel = value;
            }
        }

        public IGetTokenFromUserViewModel GetTokenFromUserViewModel
        {
            get
            {
                return _getTokenFromUserViewModel ?? (_getTokenFromUserViewModel = new GetTokenFromUserViewModel());
            }

            set
            {
                _getTokenFromUserViewModel = value;
            }
        }

        public IActivationService ActivationService { get; set; }

        public IEventAggregator EventAggregator
        {
            get
            {
                return _eventAggregator;
            }

            set
            {
                _eventAggregator = value;
                _eventAggregator.Subscribe(this);
            }
        }

        public IUIVisualizerService UiVisualizerService { get; set; }

        public IApplicationWrapper ApplicationWrapper { get; set; }

        public MainViewModel(IIOCProvider iocProvider)
            : base(iocProvider)
        {
            ActivationService = iocProvider.GetTypeFromContainer<IActivationService>();
            EventAggregator = iocProvider.GetTypeFromContainer<IEventAggregator>();
            ApplicationWrapper = iocProvider.GetTypeFromContainer<IApplicationWrapper>();
            UiVisualizerService = Resolve<IUIVisualizerService>();
        }

        public void RunActivationProcess()
        {
            ActivationService.Run();
            if (ActivationService.CurrentStep == null || ActivationService.CurrentStep.GetId().Equals(typeof(Failed)))
            {
                ApplicationWrapper.ShutDown();
            }
            else
            {
                TrayIconViewModel.Start();
            }
        }

        public void Handle(GetTokenFromUserMessage tokenRequestResutMessage)
        {
            var dispatcher = ApplicationWrapper.Dispatcher;
            var showDialogResult = dispatcher != null
                                       ? dispatcher.InvokeIfRequired((Func<bool?>)ShowGetTokenFromUserDialog)
                                       : ShowGetTokenFromUserDialog();

            var message = new TokenRequestResutMessage();
            if (showDialogResult == true)
            {
                message.Status = TokenRequestResultMessageStatusEnum.Successful;
                message.Token = GetTokenFromUserViewModel.Token;
            }
            else
            {
                message.Status = TokenRequestResultMessageStatusEnum.Canceled;
            }

            EventAggregator.Publish(message);
        }

        protected override void OnWindowLoaded()
        {
            base.OnWindowLoaded();
            Task.Factory.StartNew(RunActivationProcess);
        }

        private bool? ShowGetTokenFromUserDialog()
        {
            return UiVisualizerService.ShowDialog("GetTokenFromUser", GetTokenFromUserViewModel);
        }
    }
}