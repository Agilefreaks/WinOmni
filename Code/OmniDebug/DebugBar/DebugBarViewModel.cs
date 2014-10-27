namespace OmniDebug.DebugBar
{
    using MahApps.Metro.Controls;
    using Omni;
    using OmniUI.Flyout;

    public class DebugBarViewModel : FlyoutBaseViewModel, IDebugBarViewModel
    {
        #region Constructors and Destructors

        public DebugBarViewModel(IOmniService omniService)
        {
            OmniService = omniService;
            Position = Position.Right;
        }

        #endregion

        #region Public Properties

        public IOmniService OmniService { get; set; }

        #endregion

        #region Public Methods and Operators

        public void ShowNotification()
        {
        }

        #endregion
    }
}