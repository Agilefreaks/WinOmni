namespace OmniDebug.DebugBar
{
    using OmniCommon.Interfaces;
    using OmniDebug.Properties;
    using OmniUI.Attributes;
    using OmniUI.SecondaryMenuEntry;

    [UseView("OmniUI.SecondaryMenuEntry.SecondaryMenuEntryView", IsFullyQualifiedName = true)]
    public class DebugMenuEntryViewModel : ISecondaryMenuEntryViewModel
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
