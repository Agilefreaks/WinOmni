namespace Omnipaste.Shell.Settings
{
    using System.Threading.Tasks;
    using MahApps.Metro.Controls;
    using Ninject;
    using Omni;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Interfaces;
    using OmniUI.Flyout;
    using Omnipaste.Services;

    public class SettingsViewModel : FlyoutBaseViewModel, ISettingsViewModel
    {
        private readonly IConfigurationService _configurationService;

        #region Constructors and Destructors

        public SettingsViewModel(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
            Position = Position.Right;
        }

        #endregion

        #region Public Properties

        [Inject]
        public ISessionManager SessionManager { get; set; }

        [Inject]
        public IApplicationService ApplicationService { get; set; }

        [Inject]
        public IOmniService OmniService { get; set; }

        public override bool IsOpen
        {
            get
            {
                return base.IsOpen;
            }
            set
            {
                if (base.IsOpen != value)
                {
                    NotifyOfPropertyChange(() => IsSMSSuffixEnabled);
                }
                base.IsOpen = value;
            }
        }

        public bool IsSMSSuffixEnabled
        {
            get
            {
                return _configurationService.IsSMSSuffixEnabled;
            }
            set
            {
                if (value == _configurationService.IsSMSSuffixEnabled)
                {
                    return;
                }

                _configurationService.IsSMSSuffixEnabled = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Public Methods and Operators

        public void LogOut()
        {
            Close();
            Task.Factory.StartNew(
                () =>
                    {
                        OmniService.Stop().RunToCompletionSynchronous();
                        SessionManager.LogOut();
                    });
        }

        public void Exit()
        {
            ApplicationService.ShutDown();
        }

        #endregion

        #region Methods

        private void Close()
        {
            IsOpen = false;
        }

        #endregion
    }
}