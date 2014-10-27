namespace OmniDebug.DebugBar
{
    using MahApps.Metro.Controls;
    using Omni;
    using OmniUI.Flyout;

    public class DebugBarViewModel : FlyoutBaseViewModel, IDebugBarViewModel
    {
        public IOmniService OmniService { get; set; }

        #region Constructors and Destructors

        public DebugBarViewModel(IOmniService omniService)
        {
            OmniService = omniService;
            Position = Position.Right;
        }

        #endregion

        #region Public Methods and Operators

        public void ShowNotification()
        {
        }

        #endregion
    }
}