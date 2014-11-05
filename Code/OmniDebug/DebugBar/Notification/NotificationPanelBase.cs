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

        private readonly IEventsWrapper _eventsWrapper;

        private readonly IConnectionManagerWrapper _connectionManagerWrapper;

        private string _notificationContent;

        private string _notificationPhoneNumber;

        private DateTime _notificationTime;

        private readonly Dictionary<EventTypeEnum, string> _actionNames;

        private Dictionary<EventTypeEnum, string> _icons;

        #endregion

        #region Constructors and Destructors

        protected NotificationPanelBase(IConnectionManagerWrapper connectionManagerWrapper, IEventsWrapper eventsWrapper)
        {
            _connectionManagerWrapper = connectionManagerWrapper;
            _eventsWrapper = eventsWrapper;
            _actionNames = new Dictionary<EventTypeEnum, string> {
                                   {
                                       EventTypeEnum.IncomingCallEvent,
                                       Resources.SimulateCallNotification
                                   },
                                   {
                                       EventTypeEnum.IncomingSmsEvent,
                                       Resources.SimulateSMSNotification
                                   },
                               };
            _icons = new Dictionary<EventTypeEnum, string>
                         {
                             { EventTypeEnum.IncomingCallEvent, Resources.PhoneIcon },
                             { EventTypeEnum.IncomingSmsEvent, Resources.SMSIcon }
                         };
            NotificationTime = DateTime.Now;
        }

        #endregion

        #region Public Properties

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

        public string Icon
        {
            get
            {
                return _icons[NotificationType];
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
                        PhoneNumber = NotificationPhoneNumber
                    });
            _connectionManagerWrapper.SimulateMessage(new OmniMessage(OmniMessageTypeEnum.Notification));
        }

        #endregion
    }
}