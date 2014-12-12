namespace Omnipaste.Models
{
    using System;
    using System.Dynamic;
    using Clipboard.Models;
    using Events.Models;
    using OmniCommon.Helpers;

    public class Activity
    {
        private readonly dynamic _extraData;

        #region Constructors and Destructors

        public Activity()
        {
            Content = string.Empty;
            Time = TimeHelper.UtcNow;
            _extraData = new ExpandoObject();
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
            _extraData.ContactInfo = new ContactInfo(@event);
        }

        #endregion

        #region Public Properties

        public string Content { get; set; }

        public DateTime Time { get; set; }

        public ActivityTypeEnum Type { get; set; }

        public string Device { get; set; }

        public dynamic ExtraData
        {
            get
            {
                return _extraData;
            }
        }

        public bool WasViewed { get; set; }

        #endregion
    }
}