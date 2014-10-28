namespace OmniDebug.DebugBar
{
    using System;
    using Events.Models;
    using MahApps.Metro.Controls;
    using OmniCommon.Models;
    using OmniDebug.Services;
    using OmniUI.Flyout;

    public class DebugBarViewModel : FlyoutBaseViewModel, IDebugBarViewModel
    {
        private readonly IOmniServiceWrapper _omniServiceWrapper;

        private readonly IEventsWrapper _eventsWrapper;

        #region Constructors and Destructors

        public DebugBarViewModel(IOmniServiceWrapper omniServiceWrapper, IEventsWrapper eventsWrapper)
        {
            _omniServiceWrapper = omniServiceWrapper;
            _eventsWrapper = eventsWrapper;
            Position = Position.Right;
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods and Operators

        public void ReplayNotification()
        {
            _omniServiceWrapper.SimulateMessage(new OmniMessage(OmniMessageTypeEnum.Notification));
        }

        public void ShowNotification()
        {
            _eventsWrapper.MockLast(new Event { Content = "test", Time = DateTime.Now, Type = EventTypeEnum.IncomingSmsEvent});
            _omniServiceWrapper.SimulateMessage(new OmniMessage(OmniMessageTypeEnum.Notification));
        }

        #endregion
    }
}