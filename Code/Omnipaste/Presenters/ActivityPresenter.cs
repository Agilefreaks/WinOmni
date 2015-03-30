namespace Omnipaste.Presenters
{
    using System;
    using System.Collections.Generic;
    using Clipboard.Models;
    using Omnipaste.Models;
    using Omnipaste.Properties;
    using Omnipaste.Services;
    using OmniUI.Models;
    using OmniUI.Presenters;

    public class ActivityPresenter : Presenter, IActivityPresenterBuilder
    {
        private const string StringFormPartSeparator = " ";

        private ActivityPresenter(IModel backingModel)
            : base(backingModel)
        {
            Type = ActivityTypeEnum.None;
        }

        public string Content { get; private set; }

        public string Device { get; private set; }

        public virtual ActivityTypeEnum Type { get; private set; }

        public virtual string SourceId
        {
            get
            {
                return BackingModel.UniqueId;
            }
        }

        public ContactInfo ContactInfo { get; set; }

        #region Public Methods and Operators

        public override string ToString()
        {
            var parts = new List<string> { Device, GetTypeAsString(), Content };

            return string.Join(StringFormPartSeparator, parts);
        }

        #endregion

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

        IActivityPresenterBuilder IActivityPresenterBuilder.WithContactInfo(ContactInfo contactInfo)
        {
            ContactInfo = contactInfo;
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

        public static IActivityPresenterBuilder BeginBuild(IModel model)
        {
            return new ActivityPresenter(model);
        }
    }

    public interface IActivityPresenterBuilder
    {
        IActivityPresenterBuilder WithContactInfo(ContactInfo contactInfo);

        IActivityPresenterBuilder WithType(ActivityTypeEnum type);

        IActivityPresenterBuilder WithContent(String content);

        IActivityPresenterBuilder WithContent(UpdateInfo update);

        IActivityPresenterBuilder WithDevice(Clipping.ClippingSourceEnum clippingSourceEnum);

        IActivityPresenterBuilder WithDevice(PhoneCall phoneCall);

        IActivityPresenterBuilder WithDevice(SmsMessage smsMessage);

        ActivityPresenter Build();
    }
}