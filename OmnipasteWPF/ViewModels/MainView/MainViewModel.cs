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
    using ViewModelBase = OmnipasteWPF.ViewModels.ViewModelBase;

    public class MainViewModel : ViewModelBase, IHandle<GetTokenFromUserMessage>
    {
        private IEventAggregator _eventAggregator;

        public ITrayIconViewModel TrayIconViewModel { get; set; }

        public IGetTokenFromUserViewModel GetTokenFromUserViewModel { get; set; }

        public IActivationService ActivationService { get; set; }

        public IEventAggregator EventAggregator
        {
            get
            {
                return this._eventAggregator;
            }

            set
            {
                this._eventAggregator = value;
                this._eventAggregator.Subscribe(this);
            }
        }

        public IUIVisualizerService UiVisualizerService { get; set; }

        public IApplicationWrapper ApplicationWrapper { get; set; }

        public MainViewModel(IIOCProvider iocProvider)
            : base(iocProvider)
        {
            this.ActivationService = iocProvider.GetTypeFromContainer<IActivationService>();
            this.EventAggregator = iocProvider.GetTypeFromContainer<IEventAggregator>();
            this.ApplicationWrapper = iocProvider.GetTypeFromContainer<IApplicationWrapper>();
            this.UiVisualizerService = this.Resolve<IUIVisualizerService>();
            this.GetTokenFromUserViewModel = new GetTokenFromUserViewModel(new GetTokenFromUserIOCProvider());
            this.TrayIconViewModel = new TrayIconViewModel(new TrayIconIOCProvider());
        }

        public void RunActivationProcess()
        {
            this.ActivationService.Run();
            if (this.ActivationService.CurrentStep == null || this.ActivationService.CurrentStep.GetId().Equals(typeof(Failed)))
            {
                this.ApplicationWrapper.ShutDown();
            }
            else
            {
                this.TrayIconViewModel.TrayIconVisible = true;
            }
        }

        public void Handle(GetTokenFromUserMessage tokenRequestResutMessage)
        {
            var dispatcher = this.ApplicationWrapper.Dispatcher;
            var showDialogResult = dispatcher != null
                                       ? dispatcher.InvokeIfRequired((Func<bool?>)this.ShowGetTokenFromUserDialog)
                                       : this.ShowGetTokenFromUserDialog();

            var message = new TokenRequestResutMessage();
            if (showDialogResult == true)
            {
                message.Status = TokenRequestResultMessageStatusEnum.Successful;
                message.Token = this.GetTokenFromUserViewModel.Token;
            }
            else
            {
                message.Status = TokenRequestResultMessageStatusEnum.Canceled;
            }

            this.EventAggregator.Publish(message);
        }

        protected override void OnWindowLoaded()
        {
            base.OnWindowLoaded();
            Task.Factory.StartNew(this.RunActivationProcess);
        }

        private bool? ShowGetTokenFromUserDialog()
        {
            return this.UiVisualizerService.ShowDialog("GetTokenFromUser", this.GetTokenFromUserViewModel);
        }
    }
}