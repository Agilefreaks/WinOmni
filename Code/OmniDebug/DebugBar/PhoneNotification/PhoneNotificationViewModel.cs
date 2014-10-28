namespace OmniDebug.DebugBar.PhoneNotification
{
    using System;
    using Events.Models;
    using OmniCommon.Models;
    using OmniDebug.Services;

    public class PhoneNotificationViewModel : IDebugBarPanel
    {
        #region Fields

        private readonly IEventsWrapper _eventsWrapper;

        private readonly IOmniServiceWrapper _omniServiceWrapper;

        #endregion

        #region Constructors and Destructors

        public PhoneNotificationViewModel(IOmniServiceWrapper omniServiceWrapper, IEventsWrapper eventsWrapper)
        {
            _omniServiceWrapper = omniServiceWrapper;
            _eventsWrapper = eventsWrapper;
        }

        #endregion

        #region Public Methods and Operators

        public void SimulateCallNotification()
        {
            _eventsWrapper.MockLast(
                new Event
                    {
                        Content = "test",
                        Time = DateTime.Now,
                        Type = EventTypeEnum.IncomingCallEvent,
                        PhoneNumber = "0700123456"
                    });
            _omniServiceWrapper.SimulateMessage(new OmniMessage(OmniMessageTypeEnum.Notification));
        }

        #endregion
    }
}