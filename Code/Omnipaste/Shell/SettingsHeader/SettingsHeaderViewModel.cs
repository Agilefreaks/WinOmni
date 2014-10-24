namespace Omnipaste.Shell.SettingsHeader
{
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.Properties;
    using Omnipaste.Shell.Settings;
    using OmniUI.Attributes;

    [UseView("OmniUI.HeaderButton.HeaderButtonView", IsFullyQualifiedName = true)]
    public class SettingsHeaderViewModel : Screen, ISettingsHeaderViewModel
    {
        [Inject]
        public ISettingsViewModel SettingsViewModel { get; set; }

        public string ButtonToolTip
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
                return Resources.SettingsIconName;
            }
        }

        public void PerformAction()
        {
            SettingsViewModel.IsOpen = !SettingsViewModel.IsOpen;
        }
    }
}