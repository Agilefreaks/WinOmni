namespace Omnipaste.Shell.SettingsHeader
{
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.Shell.Settings;

    public class SettingsHeaderViewModel : Screen, ISettingsHeaderViewModel
    {
        [Inject]
        public ISettingsViewModel SettingsViewModel { get; set; }

        public void ToggleSettingsFlyout()
        {
            SettingsViewModel.IsOpen = !SettingsViewModel.IsOpen;
        }
    }
}