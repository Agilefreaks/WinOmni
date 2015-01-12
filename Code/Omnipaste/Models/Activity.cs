namespace Omnipaste.Models
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Clipboard.Models;
    using OmniCommon.Helpers;
    using Omnipaste.Properties;
    using Omnipaste.Services;

    public class Activity
    {
        #region Constants

        private const string StringFormPartSeparator = " ";

        #endregion

        #region Fields

        private readonly dynamic _extraData;

        #endregion

        #region Constructors and Destructors

        public Activity()
        {
            Content = string.Empty;
            _extraData = new ExpandoObject();
        }

        public Activity(ActivityTypeEnum activityType)
            : this()
        {
            Type = activityType;
        }

        public Activity(ClippingModel clipping)
            : this()
        {
            SourceId = clipping.UniqueId;
            Time = clipping.Time;
            Content = clipping.Content;
            Type = ActivityTypeEnum.Clipping;
            Device = clipping.Source == Clipping.ClippingSourceEnum.Cloud ? Resources.FromCloud : Resources.FromLocal;
        }

        public Activity(Call call)
            : this()
        {
            SourceId = call.UniqueId;
            Time = call.Time;
            Content = call.Content ?? string.Empty;
            Type = ActivityTypeEnum.Call;
            Device = Resources.FromCloud;
            _extraData.ContactInfo = call.ContactInfo;
        }

        public Activity(Message message)
            : this()
        {
            SourceId = message.UniqueId;
            Time = message.Time;
            Content = message.Content ?? string.Empty;
            Type = ActivityTypeEnum.Message;
            Device = Resources.FromCloud;
            _extraData.ContactInfo = message.ContactInfo;
        }

        public Activity(UpdateInfo updateInfo)
            : this()
        {
            SourceId = updateInfo.UniqueId;
            Time = updateInfo.Time;
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

        public string SourceId { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            var parts = new List<string> { Device, GetTypeAsString(), Content, GetExtraDataAsString() };

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