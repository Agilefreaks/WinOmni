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
            Position = Position.Left;
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods and Operators

        public void SimulateSmsNotification()
        {
            _eventsWrapper.MockLast(new Event { Content = "test", Time = DateTime.Now, Type = EventTypeEnum.IncomingSmsEvent, PhoneNumber = "0700123456" });
            _omniServiceWrapper.SimulateMessage(new OmniMessage(OmniMessageTypeEnum.Notification));
        }
        
        public void SimulateCallNotification()
        {
            _eventsWrapper.MockLast(new Event { Content = "test", Time = DateTime.Now, Type = EventTypeEnum.IncomingCallEvent, PhoneNumber = "0700123456"});
            _omniServiceWrapper.SimulateMessage(new OmniMessage(OmniMessageTypeEnum.Notification));
        }

        #endregion
    }
}