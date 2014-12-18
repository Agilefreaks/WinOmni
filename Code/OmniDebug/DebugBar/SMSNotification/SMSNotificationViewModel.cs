namespace OmniDebug.DebugBar.SMSNotification
{
    using Events.Models;
    using OmniDebug.DebugBar.Notification;
    using OmniDebug.Services;
    using OmniUI.Attributes;

    [UseView(typeof(SMSNotificationView))]
    public class SMSNotificationViewModel : NotificationPanelBase, IDebugBarPanel
    {
        #region Constructors and Destructors

        public SMSNotificationViewModel(IOmniServiceWrapper omniServiceWrapper, IEventsWrapper eventsWrapper)
            : base(omniServiceWrapper, eventsWrapper)
        {
        }

        #endregion

        #region Properties

        protected override EventTypeEnum NotificationType
        {
            get
            {
                return EventTypeEnum.IncomingSmsEvent;
            }
        }

        #endregion
    }
}