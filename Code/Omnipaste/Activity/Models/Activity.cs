namespace Omnipaste.Activity.Models
{
    using System;
    using Clipboard.Models;
    using Events.Models;
    using OmniCommon.Helpers;

    public class Activity
    {
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
            Device = clipping.Source == Clipping.ClippingSourceEnum.Cloud
                         ? Properties.Resources.FromCloud
                         : Properties.Resources.FromLocal;
        }

        public Activity(Event @event)
            : this()
        {
            Content = @event.Content ?? string.Empty;
            Type = @event.Type == EventTypeEnum.IncomingCallEvent ? ActivityTypeEnum.Call : ActivityTypeEnum.Message;
            Device = Properties.Resources.FromCloud;
            ContactInfo = new ContactInfo(@event);
        }

        #endregion

        #region Public Properties

        public string Content { get; set; }

        public DateTime Time { get; set; }

        public ActivityTypeEnum Type { get; set; }

        public string Device { get; set; }

        public ContactInfo ContactInfo { get; set; }

        #endregion
    }
}