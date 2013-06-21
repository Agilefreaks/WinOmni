namespace OmnipasteWPF.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using Caliburn.Micro;
    using Cinch;
    using OmniCommon.EventAggregatorMessages;
    using OmniCommon.Interfaces;

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

        public MainViewModel(IIOCProvider iocProvider)
            : base(iocProvider)
        {
            ActivationService = iocProvider.GetTypeFromContainer<IActivationService>();
            EventAggregator = iocProvider.GetTypeFromContainer<IEventAggregator>();
            UiVisualizerService = Resolve<IUIVisualizerService>();
            GetTokenFromUserViewModel = new GetTokenFromUserViewModel(new GetTokenFromUserIOCProvider());
        }

        public void StartActivationProcess()
        {
            ActivationService.Run();
        }

        public void Handle(GetTokenFromUserMessage tokenRequestResutMessage)
        {
            var dispatcher = GetDispatcher();
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
            Task.Factory.StartNew(StartActivationProcess);
        }

        private static Dispatcher GetDispatcher()
        {
            return Application.Current != null ? Application.Current.Dispatcher : null;
        }

        private bool? ShowGetTokenFromUserDialog()
        {
            return UiVisualizerService.ShowDialog("GetTokenFromUser", GetTokenFromUserViewModel);
        }
    }
}