namespace Omnipaste.Shell.SettingsHeader
{
    using Caliburn.Micro;
    using Omnipaste.Shell.Settings;
    using OmniUI.HeaderButton;

    public interface ISettingsHeaderViewModel : IScreen, IHeaderButtonViewModel
    {
        ISettingsViewModel SettingsViewModel { get; set; }
    }
}