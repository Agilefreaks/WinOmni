namespace Omnipaste.Presenters
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Caliburn.Micro;
    using Clipboard.Models;
    using Omnipaste.Models;
    using Omnipaste.Properties;
    using Omnipaste.Services;
    using Message = Omnipaste.Models.Message;

    public class ActivityPresenter : PropertyChangedBase
    {
        #region Constants

        private const string StringFormPartSeparator = " ";

        #endregion

        #region Constructors and Destructors

        public ActivityPresenter()
        {
            Type = ActivityTypeEnum.None;
            ExtraData = new ExpandoObject();
        }

        public ActivityPresenter(ActivityTypeEnum type)
            : this()
        {
            Type = type;
        }

        public ActivityPresenter(ClippingModel clipping)
            : this()
        {
            BackingModel = clipping;
            Content = clipping.Content;
            Type = ActivityTypeEnum.Clipping;
            Device = clipping.Source == Clipping.ClippingSourceEnum.Cloud ? Resources.FromCloud : Resources.FromLocal;
        }

        public ActivityPresenter(Call call)
            : this()
        {
            BackingModel = call;
            Content = call.Content ?? string.Empty;
            Type = ActivityTypeEnum.Call;
            Device = Resources.FromCloud;
            ExtraData.ContactInfo = call.ContactInfo;
        }

        public ActivityPresenter(Message message)
            : this()
        {
            BackingModel = message;
            Content = message.Content ?? string.Empty;
            Type = ActivityTypeEnum.Message;
            Device = Resources.FromCloud;
            ExtraData.ContactInfo = message.ContactInfo;
        }

        public ActivityPresenter(UpdateInfo updateInfo)
            : this()
        {
            BackingModel = updateInfo;
            Content = updateInfo.WasInstalled ? Resources.NewVersionInstalled : Resources.NewVersionAvailable;
            Type = ActivityTypeEnum.Version;
            ExtraData.UpdateInfo = updateInfo;
        }

        #endregion

        #region Public Properties

        public BaseModel BackingModel { get; private set; }

        public string Content { get; private set; }

        public string Device { get; private set; }

        public ActivityTypeEnum Type { get; private set; }

        public dynamic ExtraData { get; private set; }

        public DateTime Time
        {
            get
            {
                return BackingModel.Time;
            }
        }

        public bool WasViewed
        {
            get
            {
                return BackingModel.WasViewed;
            }
            set
            {
                if (value.Equals(BackingModel.WasViewed))
                {
                    return;
                }
                BackingModel.WasViewed = value;
                NotifyOfPropertyChange(() => WasViewed);
            }
        }

        public string SourceId
        {
            get
            {
                return BackingModel.UniqueId;
            }
        }

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