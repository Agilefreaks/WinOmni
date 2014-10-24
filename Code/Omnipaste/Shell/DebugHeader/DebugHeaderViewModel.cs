namespace Omnipaste.Shell.DebugHeader
{
    using Ninject;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using Omnipaste.Shell.Debug;

    public class DebugHeaderViewModel : IDebugHeaderViewModel
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        #endregion

        #region Constructors and Destructors

        public DebugHeaderViewModel(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        #endregion

        #region Public Properties

        [Inject]
        public IDebugBarViewModel DebugBarViewModel { get; set; }

        public bool IsAvailable
        {
            get
            {
                bool value;
                return bool.TryParse(_configurationService[ConfigurationProperties.ShowDebugBar], out value) && value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void ToggleDebugBarFlyout()
        {
            DebugBarViewModel.IsOpen = !DebugBarViewModel.IsOpen;
        }

        #endregion
    }
}