namespace Omnipaste.Shell
{
    using System;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.EventAggregatorMessages;
    using OmniCommon.Interfaces;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;
    using Omnipaste.Framework;
    using Omnipaste.UserToken;

    public class ShellViewModel : Conductor<IWorkspace>.Collection.OneActive, IShellViewModel, IHandle<GetTokenFromUserMessage>
    {
        private readonly IActivationService _activationService;

        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public IApplicationWrapper ApplicationWrapper { get; set; }

        [Inject]
        public IWindowManager WindowManager { get; set; }

        [Inject]
        public IUserTokenViewModel UserTokenViewModel { get; set; }

        public ShellViewModel(IActivationService activationService, IEventAggregator eventAggregator)
        {
            _activationService = activationService;

            ApplicationWrapper = new ApplicationWrapper();
            
            eventAggregator.Subscribe(this);
        }

        public void Handle(GetTokenFromUserMessage message)
        {
            WindowManager.ShowDialog(UserTokenViewModel);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _activationService.Run();

            if (_activationService.CurrentStep == null || _activationService.CurrentStep.GetId().Equals(typeof(Failed)))
            {
                ApplicationWrapper.ShutDown();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}