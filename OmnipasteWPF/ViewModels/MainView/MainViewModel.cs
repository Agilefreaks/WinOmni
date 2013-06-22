﻿namespace OmnipasteWPF.ViewModels.MainView
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
                this._trayIconViewModel = value;
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
                TrayIconViewModel.Start();
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