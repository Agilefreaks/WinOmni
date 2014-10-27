namespace OmniDebug.DebugHeader
{
    using Ninject;
    using OmniCommon.Interfaces;
    using OmniDebug.DebugBar;
    using OmniDebug.Properties;
    using OmniUI.Attributes;

    [UseView("OmniUI.HeaderButton.HeaderButtonView", IsFullyQualifiedName = true)]
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

        public string ButtonToolTip
        {
            get
            {
                return Resources.Debug;
            }
        }

        public string Icon
        {
            get
            {
                return Resources.DebugIconName;
            }
        }

        public bool IsAvailable
        {
            get
            {
                return _configurationService.DebugMode;
            }
        }

        [Inject]
        public IDebugBarViewModel DebugBarViewModel { get; set; }

        #endregion

        #region Public Methods and Operators

        public void OnButtonClick()
        {
            DebugBarViewModel.IsOpen = !DebugBarViewModel.IsOpen;
        }

        #endregion
    }
}