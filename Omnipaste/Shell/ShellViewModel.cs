namespace Omnipaste.Shell
{
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.Configuration;
    using Omnipaste.Framework;
    using Omnipaste.Properties;
    using Omnipaste.UserToken;

    public class ShellViewModel : Conductor<IWorkspace>.Collection.OneActive, IShellViewModel
    {
        [Inject]
        public IWindowManager WindowManager { get; set; }

        [Inject]
        public IUserTokenViewModel UserTokenViewModel { get; set; }

        public IConfigurationViewModel ConfigurationViewModel { get; set; }

        public ShellViewModel(IConfigurationViewModel configurationViewModel, IEventAggregator eventAggregator)
        {
            ConfigurationViewModel = configurationViewModel;

            DisplayName = Resources.AplicationName;

            eventAggregator.Subscribe(this);
        }

        public void Handle(GetTokenFromUserMessage message)
        {
            ActiveItem = UserTokenViewModel;
        }

        public void Handle(TokenRequestResultMessage message)
        {
            ActiveItem = ConfigurationViewModel;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            ActiveItem = ConfigurationViewModel;
            ConfigurationViewModel.Start();
        }
    }
}