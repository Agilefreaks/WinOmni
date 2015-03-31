namespace Omnipaste.Shell.Settings
{
    using Caliburn.Micro;
    using Omnipaste.Properties;
    using OmniUI.Attributes;
    using OmniUI.Menu.SecondaryItem;

    [UseView(typeof(SecondaryItemView))]
    public class SettingsItemViewModel : Screen, ISecondaryItemViewModel
    {
        #region Fields

        private readonly ISettingsViewModel _settingsViewModel;

        #endregion

        #region Constructors and Destructors

        public SettingsItemViewModel(ISettingsViewModel settingsViewModel)
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

        public override string DisplayName
        {
            get
            {
                return Resources.Settings;
            }
        }

        #endregion

        #region Public Methods

        public void PerformAction()
        {
            _settingsViewModel.IsOpen = !_settingsViewModel.IsOpen;
        }

        #endregion

        public void Dispose()
        {
        }
    }
}
