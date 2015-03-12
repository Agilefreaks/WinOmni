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

    public class ActivityPresenter : PropertyChangedBase, IActivityPresenterBuilder
    {
        private const string StringFormPartSeparator = " ";

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

        public virtual IPresenter BackingModel { get; private set; }

        public string Content { get; private set; }

        public string Device { get; private set; }

        public virtual ActivityTypeEnum Type { get; private set; }

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
                NotifyOfPropertyChange();
            }
        }

        public virtual string SourceId
        {
            get
            {
                return BackingModel.UniqueId;
            }
        }

        public bool IsDeleted
        {
            get
            {
                return BackingModel.IsDeleted;
            }
            set
            {
                if (value.Equals(BackingModel.IsDeleted))
                {
                    return;
                }
                BackingModel.IsDeleted = value;
                NotifyOfPropertyChange();
            }
        }

        #region Public Methods and Operators

        public override string ToString()
        {
            var parts = new List<string> { Device, GetTypeAsString(), Content, GetExtraDataAsString() };

            return string.Join(StringFormPartSeparator, parts);
        }

        #endregion

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

        IActivityPresenterBuilder IActivityPresenterBuilder.WithBackingModel(IPresenter backingModel)
        {
            BackingModel = backingModel;
            return this;
        }

        IActivityPresenterBuilder IActivityPresenterBuilder.WithBackingModel(IConversationPresenter backingModel)
        {
            BackingModel = backingModel;
            ExtraData.ContactInfo = backingModel.ContactInfoPresenter;
            return this;
        }

        IActivityPresenterBuilder IActivityPresenterBuilder.WithBackingModel(UpdateInfo backingModel)
        {
            ExtraData.UpdateInfo = backingModel;
            return this;
        }


        IActivityPresenterBuilder IActivityPresenterBuilder.WithType(ActivityTypeEnum type)
        {
            Type = type;
            return this;
        }

        IActivityPresenterBuilder IActivityPresenterBuilder.WithContent(String content)
        {
            Content = content;
            return this;
        }

        IActivityPresenterBuilder IActivityPresenterBuilder.WithContent(UpdateInfo updateInfo)
        {
            Content = updateInfo.WasInstalled ? Resources.NewVersionInstalled : Resources.NewVersionAvailable;
            return this;
        }

        IActivityPresenterBuilder IActivityPresenterBuilder.WithDevice(Clipping.ClippingSourceEnum clippingSourceEnum)
        {
             Device = clippingSourceEnum == Clipping.ClippingSourceEnum.Cloud ? Resources.FromCloud : Resources.FromLocal;
            return this;
        }

        IActivityPresenterBuilder IActivityPresenterBuilder.WithDevice(PhoneCall phoneCall)
        {
            Device = phoneCall is RemotePhoneCall ? Resources.FromCloud : Resources.FromLocal;
            return this;
        }

        IActivityPresenterBuilder IActivityPresenterBuilder.WithDevice(SmsMessage smsMessage)
        {
            Device = smsMessage is RemoteSmsMessage ? Resources.FromCloud : Resources.FromLocal;
            return this;
        }

        public ActivityPresenter Build()
        {
            return this;
        }

        public static IActivityPresenterBuilder BeginBuild()
        {
            return new ActivityPresenter();
        }
    }

    public interface IActivityPresenterBuilder
    {
        IActivityPresenterBuilder WithBackingModel(IPresenter backingModel);

        // TODO: add a presenter
        IActivityPresenterBuilder WithBackingModel(UpdateInfo backingModel);

        IActivityPresenterBuilder WithBackingModel(IConversationPresenter backingModel);

        IActivityPresenterBuilder WithType(ActivityTypeEnum type);

        IActivityPresenterBuilder WithContent(String content);

        IActivityPresenterBuilder WithContent(UpdateInfo update);

        IActivityPresenterBuilder WithDevice(Clipping.ClippingSourceEnum clippingSourceEnum);

        IActivityPresenterBuilder WithDevice(PhoneCall phoneCall);

        IActivityPresenterBuilder WithDevice(SmsMessage smsMessage);

        ActivityPresenter Build();
    }
}