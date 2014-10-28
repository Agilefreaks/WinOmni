namespace OmniDebug.DebugBar.SMSNotification
{
    using System;
    using Events.Models;
    using OmniCommon.Models;
    using OmniDebug.Services;

    public class SMSNotificationViewModel : IDebugBarPanel
    {
        #region Fields

        private readonly IEventsWrapper _eventsWrapper;

        private readonly IOmniServiceWrapper _omniServiceWrapper;

        #endregion

        #region Constructors and Destructors

        public SMSNotificationViewModel(IOmniServiceWrapper omniServiceWrapper, IEventsWrapper eventsWrapper)
        {
            _omniServiceWrapper = omniServiceWrapper;
            _eventsWrapper = eventsWrapper;
        }

        #endregion

        #region Public Methods and Operators

        public void SimulateSmsNotification()
        {
            _eventsWrapper.MockLast(
                new Event
                    {
                        Content = "test",
                        Time = DateTime.Now,
                        Type = EventTypeEnum.IncomingSmsEvent,
                        PhoneNumber = "0700123456"
                    });
            _omniServiceWrapper.SimulateMessage(new OmniMessage(OmniMessageTypeEnum.Notification));
        }

        #endregion
    }
}