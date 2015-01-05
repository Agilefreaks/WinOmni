namespace OmniDebug.DebugBar.Notification
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using Events.Models;
    using OmniCommon.Models;
    using OmniDebug.Properties;
    using OmniDebug.Services;

    public abstract class NotificationPanelBase : PropertyChangedBase
    {
        #region Fields

        private readonly Dictionary<EventTypeEnum, string> _actionNames;

        private readonly IEventsWrapper _eventsWrapper;

        private readonly Dictionary<EventTypeEnum, string> _icons;

        private readonly IOmniServiceWrapper _omniServiceWrapper;

        private string _notificationContactName;

        private string _notificationContent;

        private string _notificationPhoneNumber;

        private DateTime _notificationTime;
        #endregion

        #region Constructors and Destructors

        protected NotificationPanelBase(IOmniServiceWrapper omniServiceWrapper, IEventsWrapper eventsWrapper)
        {
            _omniServiceWrapper = omniServiceWrapper;
            _eventsWrapper = eventsWrapper;
            _actionNames = new Dictionary<EventTypeEnum, string>
                               {
                                   { EventTypeEnum.IncomingCallEvent, Resources.SimulateCallNotification },
                                   { EventTypeEnum.IncomingSmsEvent, Resources.SimulateSMSNotification },
                               };
            _icons = new Dictionary<EventTypeEnum, string>
                         {
                             { EventTypeEnum.IncomingCallEvent, OmniUI.Resources.IconNames.Phone },
                             { EventTypeEnum.IncomingSmsEvent, OmniUI.Resources.IconNames.SMS }
                         };
            NotificationTime = DateTime.Now;
            NotificationContactName = "Some Contact";
            NotificationContent = "some content";
            NotificationPhoneNumber = "0788999666";
            NotificationTime = DateTime.Now;
        }

        #endregion

        #region Public Properties

        public virtual bool CanAddContent
        {
            get
            {
                return true;
            }
        }

        public string Icon
        {
            get
            {
                return _icons[NotificationType];
            }
        }

        public string NotificationContactName
        {
            get
            {
                return _notificationContactName;
            }
            set
            {
                if (value == _notificationContactName)
                {
                    return;
                }
                _notificationContactName = value;
                NotifyOfPropertyChange();
            }
        }

        public string NotificationContent
        {
            get
            {
                return _notificationContent;
            }
            set
            {
                if (value == _notificationContent)
                {
                    return;
                }
                _notificationContent = value;
                NotifyOfPropertyChange();
            }
        }

        public string NotificationPhoneNumber
        {
            get
            {
                return _notificationPhoneNumber;
            }
            set
            {
                if (value == _notificationPhoneNumber)
                {
                    return;
                }
                _notificationPhoneNumber = value;
                NotifyOfPropertyChange();
            }
        }

        public DateTime NotificationTime
        {
            get
            {
                return _notificationTime;
            }
            set
            {
                if (value == _notificationTime)
                {
                    return;
                }
                _notificationTime = value;
                NotifyOfPropertyChange();
            }
        }

        public string PerformActionName
        {
            get
            {
                return _actionNames[NotificationType];
            }
        }

        #endregion

        #region Properties

        protected abstract EventTypeEnum NotificationType { get; }

        #endregion

        #region Public Methods and Operators

        public void SimulateNotification()
        {
            _eventsWrapper.MockLast(
                new Event
                    {
                        Content = NotificationContent,
                        Time = NotificationTime,
                        Type = NotificationType,
                        PhoneNumber = NotificationPhoneNumber,
                        ContactName = NotificationContactName
                    });
            _omniServiceWrapper.SimulateMessage(new OmniMessage(OmniMessageTypeEnum.Notification));
        }

        #endregion
    }
}