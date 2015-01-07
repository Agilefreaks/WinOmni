﻿namespace Omnipaste.Models
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Threading;
    using Clipboard.Models;
    using Events.Models;
    using OmniCommon.Helpers;
    using Omnipaste.Properties;
    using Omnipaste.Services;

    public class Activity
    {
        #region Constants

        private const string DateTimeFormatPattern = "yyyy-M-d MMMM dddd";

        private const string StringFormPartSeparator = " ";

        #endregion

        #region Fields

        private readonly dynamic _extraData;

        #endregion

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
            Device = clipping.Source == Clipping.ClippingSourceEnum.Cloud ? Resources.FromCloud : Resources.FromLocal;
        }

        public Activity(Event @event)
            : this()
        {
            Content = @event.Content ?? string.Empty;
            Type = @event.Type == EventTypeEnum.IncomingCallEvent ? ActivityTypeEnum.Call : ActivityTypeEnum.Message;
            Device = Resources.FromCloud;
            _extraData.ContactInfo = new EventContactInfo(@event);
        }

        public Activity(UpdateInfo updateInfo)
            : this()
        {
            Content = updateInfo.WasInstalled ? Resources.NewVersionInstalled : Resources.NewVersionAvailable;
            Type = ActivityTypeEnum.Version;
            _extraData.UpdateInfo = updateInfo;
        }

        #endregion

        #region Public Properties

        public string Content { get; set; }

        public string Device { get; set; }

        public dynamic ExtraData
        {
            get
            {
                return _extraData;
            }
        }

        public DateTime Time { get; set; }

        public ActivityTypeEnum Type { get; set; }

        public bool WasViewed { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            var parts = new List<string>
                            {
                                Device,
                                GetTypeAsString(),
                                Content,
                                GetExtraDataAsString()
                            };

            return string.Join(StringFormPartSeparator, parts);
        }

        #endregion

        #region Methods

        private string GetExtraDataAsString()
        {
            var dictionary = ((IDictionary<string, object>)ExtraData);
            return dictionary.Keys.Aggregate(
                string.Empty,
                (currentValue, key) => currentValue + (StringFormPartSeparator + dictionary[key].ToString()));
        }

        private string GetTypeAsString()
        {
            string result;
            switch (Type)
            {
                case ActivityTypeEnum.Call:
                    result = Resources.CallLabel;
                    break;
                case ActivityTypeEnum.Message:
                    result = Resources.SMSLabel;
                    break;
                default:
                    result = Type.ToString();
                    break;
            }
            return result;
        }
        #endregion
    }
}