namespace OmniDebug.DebugBar
{
    using Caliburn.Micro;
    using OmniApi.Resources;
    using OmniCommon.Interfaces;
    using OmniDebug.Properties;
    using OmniUI.Attributes;
    using OmniUI.SecondaryMenuEntry;

    [UseView(typeof(SecondaryMenuEntryView))]
    public class DebugMenuEntryViewModel : Screen, ISecondaryMenuEntryViewModel
    {
        #region Fields

        private readonly IConfigurationService _configurationService;
        private readonly IDebugBarViewModel _debugBarViewModel;

        #endregion

        #region Public Properties
		
        public string ToolTipText
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
                return OmniUI.Resources.IconNames.SideMenuDebug;
            }
        }

        public bool CanPerformAction
        {
            get
            {
                return _configurationService.DebugMode;
            }
        }

        public override string DisplayName
        {
            get
            {
                return Resources.Debug;
            }
        }

        #endregion

        #region Constructors and Destructors

        public DebugMenuEntryViewModel(IConfigurationService configurationService, IDebugBarViewModel debugBarViewModel)
        {
            _configurationService = configurationService;
            _debugBarViewModel = debugBarViewModel;
        }

        #endregion

        #region Public Methods

        public void PerformAction()
        {
            _debugBarViewModel.IsOpen = !_debugBarViewModel.IsOpen;
        }

        #endregion

        public void Dispose()
        {
        }
    }
}
