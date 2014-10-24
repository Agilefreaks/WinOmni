namespace OmniDebug.DebugBar
{
    using MahApps.Metro.Controls;
    using OmniCommon.Models;
    using OmniDebug.Services;
    using OmniUI.Flyout;

    public class DebugBarViewModel : FlyoutBaseViewModel, IDebugBarViewModel
    {
        #region Constructors and Destructors

        public DebugBarViewModel(IOmniServiceWrapper omniServiceWrapper)
        {
            OmniServiceWrapper = omniServiceWrapper;
            Position = Position.Right;
        }

        #endregion

        #region Public Properties

        public IOmniServiceWrapper OmniServiceWrapper { get; set; }

        #endregion

        #region Public Methods and Operators

        public void ShowNotification()
        {
            OmniServiceWrapper.SimulateMessage(new OmniMessage(OmniMessageTypeEnum.Notification));
        }

        #endregion
    }
}