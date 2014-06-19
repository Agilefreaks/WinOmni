namespace Omnipaste.Shell.Settings
{
    using MahApps.Metro.Controls;
    using Omnipaste.Framework;

    public class SettingsViewModel : FlyoutBaseViewModel, ISettingsViewModel
    {
        public SettingsViewModel()
        {
            Position = Position.Right;
        }
    }
}