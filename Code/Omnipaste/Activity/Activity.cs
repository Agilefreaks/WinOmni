﻿namespace Omnipaste.Activity
{
    using System;
    using Clipboard.Models;
    using Events.Models;
    using OmniCommon.Helpers;

    public class Activity
    {
        #region Constants

        private const string EventContentFormat = "{0} at {1} from {2} {3}";

        #endregion

        #region Constructors and Destructors

        public Activity()
        {
            Content = string.Empty;
            Time = TimeHelper.UtcNow;
        }

        public Activity(ActivityTypeEnum activityType)
            : this()
        {
            Type = activityType;
        }

        public Activity(Clipping clipping)
            : this()
        {
            Content = clipping.Content;
            Type = ActivityTypeEnum.Clipping;
        }

        public Activity(Event @event)
            : this()
        {
            Content =
                string.Format(
                    EventContentFormat,
                    @event.Type,
                    @event.Time,
                    string.IsNullOrWhiteSpace(@event.ContactName) ? @event.PhoneNumber : @event.ContactName,
                    @event.Content).Trim();
            Type = @event.Type == EventTypeEnum.IncomingCallEvent ? ActivityTypeEnum.Call : ActivityTypeEnum.Message;
        }

        #endregion

        #region Public Properties

        public string Content { get; private set; }

        public DateTime Time { get; set; }

        public ActivityTypeEnum Type { get; set; }

        #endregion
    }
}