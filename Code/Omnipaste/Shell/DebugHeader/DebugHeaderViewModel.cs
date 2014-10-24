namespace Omnipaste.Shell.DebugHeader
{
    using Ninject;
    using Omnipaste.Shell.Debug;

    public class DebugHeaderViewModel : IDebugHeaderViewModel
    {
        [Inject]
        public IDebugBarViewModel DebugBarViewModel { get; set; }

        public void ToggleDebugBarFlyout()
        {
            DebugBarViewModel.IsOpen = !DebugBarViewModel.IsOpen;
        }
    }
}