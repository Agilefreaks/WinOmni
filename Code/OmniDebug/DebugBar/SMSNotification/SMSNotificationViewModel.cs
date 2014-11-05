namespace OmniDebug.DebugBar.SMSNotification
{
    using Events.Models;
    using OmniDebug.DebugBar.Notification;
    using OmniDebug.Services;
    using OmniUI.Attributes;

    [UseView("OmniDebug.DebugBar.SMSNotification.SMSNotificationView", IsFullyQualifiedName = true)]
    public class SMSNotificationViewModel : NotificationPanelBase, IDebugBarPanel
    {
        #region Constructors and Destructors

        public SMSNotificationViewModel(IConnectionManagerWrapper connectionManagerWrapper, IEventsWrapper eventsWrapper)
            : base(connectionManagerWrapper, eventsWrapper)
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