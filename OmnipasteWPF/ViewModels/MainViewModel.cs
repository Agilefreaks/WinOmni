namespace OmnipasteWPF.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Cinch;
    using OmniCommon.EventAggregatorMessages;
    using OmniCommon.Interfaces;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;
    using OmnipasteWPF.DataProviders;

    public class MainViewModel : ViewModelBase, IHandle<GetTokenFromUserMessage>
    {
        private IEventAggregator _eventAggregator;

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

        public IGetTokenFromUserViewModel GetTokenFromUserViewModel { get; set; }

        public IApplicationWrapper ApplicationWrapper { get; set; }

        public MainViewModel(IIOCProvider iocProvider)
            : base(iocProvider)
        {
            ActivationService = iocProvider.GetTypeFromContainer<IActivationService>();
            EventAggregator = iocProvider.GetTypeFromContainer<IEventAggregator>();
            ApplicationWrapper = iocProvider.GetTypeFromContainer<IApplicationWrapper>();
            UiVisualizerService = Resolve<IUIVisualizerService>();
            GetTokenFromUserViewModel = new GetTokenFromUserViewModel(new GetTokenFromUserIOCProvider());
        }

        public void RunActivationProcess()
        {
            ActivationService.Run();
            if (ActivationService.CurrentStep == null || ActivationService.CurrentStep.GetId().Equals(typeof(Failed)))
            {
                ApplicationWrapper.ShutDown();
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