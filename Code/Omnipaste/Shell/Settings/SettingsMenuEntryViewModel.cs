namespace Omnipaste.Shell.Settings
{
    using Omnipaste.Properties;
    using OmniUI.Attributes;
    using OmniUI.Intefaces;

    [UseView("OmniUI.SecondaryMenuEntry.SecondaryMenuEntryView", IsFullyQualifiedName = true)]
    public class SettingsMenuEntryViewModel : ISecondaryMenuEntryViewModel
    {
        #region Fields

        private readonly ISettingsViewModel _settingsViewModel;

        #endregion

        #region Constructors and Destructors

        public SettingsMenuEntryViewModel(ISettingsViewModel settingsViewModel)
        {
            _settingsViewModel = settingsViewModel;
        }

        #endregion

        #region Public Properties

        public string ToolTipText
        {
            get
            {
                return Resources.SettingsHeader;
            }
        }

        public string Icon
        {
            get
            {
                return OmniUI.Resources.IconNames.SideMenuSettings;
            }
        }

        public bool CanPerformAction
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Public Methods

        public void PerformAction()
        {
            _settingsViewModel.IsOpen = !_settingsViewModel.IsOpen;
        }

        #endregion
    }
}
