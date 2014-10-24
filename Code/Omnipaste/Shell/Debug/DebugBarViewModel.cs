namespace Omnipaste.Shell.Debug
{
    using MahApps.Metro.Controls;
    using Omnipaste.Framework;

    public class DebugBarViewModel : FlyoutBaseViewModel, IDebugBarViewModel
    {
        public DebugBarViewModel()
        {
            Position = Position.Right;
        }
    }
}