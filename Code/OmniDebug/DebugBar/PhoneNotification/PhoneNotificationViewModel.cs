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

        public PhoneNotificationViewModel(IOmniServiceWrapper omniServiceWrapper, IEventsWrapper eventsWrapper)
            : base(omniServiceWrapper, eventsWrapper)
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

        public override bool CanAddContent
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}