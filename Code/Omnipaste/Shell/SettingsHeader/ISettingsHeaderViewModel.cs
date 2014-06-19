namespace Omnipaste.Shell.SettingsHeader
{
    using Caliburn.Micro;
    using Omnipaste.Shell.Settings;

    public interface ISettingsHeaderViewModel : IScreen
    {
        ISettingsViewModel SettingsViewModel { get; set; }

        void ToggleSettingsFlyout();
    }
}