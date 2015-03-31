namespace Omnipaste.Presenters
{
    using System;
    using System.Collections.Generic;
    using Clipboard.Dto;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Properties;
    using Omnipaste.Services;
    using OmniUI.Entities;
    using OmniUI.Presenters;

    public class ActivityPresenter : Presenter, IActivityPresenterBuilder
    {
        private const string StringFormPartSeparator = " ";

        private ActivityPresenter(IEntity backingModel)
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

        public ContactEntity ContactEntity { get; set; }

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

        IActivityPresenterBuilder IActivityPresenterBuilder.WithContactInfo(ContactEntity contactEntity)
        {
            ContactEntity = contactEntity;
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

        IActivityPresenterBuilder IActivityPresenterBuilder.WithDevice(ClippingDto.ClippingSourceEnum clippingSourceEnum)
        {
            Device = clippingSourceEnum == ClippingDto.ClippingSourceEnum.Cloud ? Resources.FromCloud : Resources.FromLocal;
            return this;
        }

        IActivityPresenterBuilder IActivityPresenterBuilder.WithDevice(PhoneCallEntity phoneCallEntity)
        {
            Device = phoneCallEntity is RemotePhoneCallEntity ? Resources.FromCloud : Resources.FromLocal;
            return this;
        }

        IActivityPresenterBuilder IActivityPresenterBuilder.WithDevice(SmsMessageEntity smsMessageEntity)
        {
            Device = smsMessageEntity is RemoteSmsMessageEntity ? Resources.FromCloud : Resources.FromLocal;
            return this;
        }

        public ActivityPresenter Build()
        {
            return this;
        }

        public static IActivityPresenterBuilder BeginBuild(IEntity entity)
        {
            return new ActivityPresenter(entity);
        }
    }

    public interface IActivityPresenterBuilder
    {
        IActivityPresenterBuilder WithContactInfo(ContactEntity contactEntity);

        IActivityPresenterBuilder WithType(ActivityTypeEnum type);

        IActivityPresenterBuilder WithContent(String content);

        IActivityPresenterBuilder WithContent(UpdateInfo update);

        IActivityPresenterBuilder WithDevice(ClippingDto.ClippingSourceEnum clippingSourceEnum);

        IActivityPresenterBuilder WithDevice(PhoneCallEntity phoneCallEntity);

        IActivityPresenterBuilder WithDevice(SmsMessageEntity smsMessageEntity);

        ActivityPresenter Build();
    }
}