namespace OmniDebug.DebugBar.PhoneNotification
{
    using Events.Models;
    using OmniDebug.DebugBar.Notification;
    using OmniDebug.Services;
    using OmniUI.Attributes;

    [UseView("OmniDebug.DebugBar.SMSNotification.SMSNotificationView", IsFullyQualifiedName = true)]
    public class PhoneNotificationViewModel : NotificationPanelBase, IDebugBarPanel
    {
        #region Constructors and Destructors

        public PhoneNotificationViewModel(IConnectionManagerWrapper connectionManagerWrapper, IEventsWrapper eventsWrapper)
            : base(connectionManagerWrapper, eventsWrapper)
        {
        }

        #endregion

        #region Properties

        protected override EventTypeEnum NotificationType
        {
            get
            {
                return EventTypeEnum.IncomingCallEvent;
            }
        }

        #endregion
    }
}