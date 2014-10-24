namespace Omnipaste.Shell.Debug
{
    using MahApps.Metro.Controls;
    using Omni;
    using Omnipaste.Framework;

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