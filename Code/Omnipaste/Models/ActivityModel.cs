namespace Omnipaste.Models
{
    using System;
    using System.Collections.Generic;
    using Clipboard.Dto;
    using Omnipaste.Activity;
    using Omnipaste.Entities;
    using Omnipaste.Properties;
    using OmniUI.Entities;
    using OmniUI.Models;

    public class ActivityModel : Model, IActivityModelBuilder
    {
        private const string StringFormPartSeparator = " ";

        private ActivityModel(IEntity backingEntity)
            : base(backingEntity)
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
                return BackingEntity.UniqueId;
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

        IActivityModelBuilder IActivityModelBuilder.WithContact(ContactEntity contactEntity)
        {
            ContactEntity = contactEntity;
            return this;
        }

        IActivityModelBuilder IActivityModelBuilder.WithType(ActivityTypeEnum type)
        {
            Type = type;
            return this;
        }

        IActivityModelBuilder IActivityModelBuilder.WithContent(String content)
        {
            Content = content;
            return this;
        }

        IActivityModelBuilder IActivityModelBuilder.WithContent(UpdateEntity updateEntity)
        {
            Content = updateEntity.WasInstalled ? Resources.NewVersionInstalled : Resources.NewVersionAvailable;
            return this;
        }

        IActivityModelBuilder IActivityModelBuilder.WithDevice(ClippingDto.ClippingSourceEnum clippingSourceEnum)
        {
            Device = clippingSourceEnum == ClippingDto.ClippingSourceEnum.Cloud ? Resources.FromCloud : Resources.FromLocal;
            return this;
        }

        IActivityModelBuilder IActivityModelBuilder.WithDevice(PhoneCallEntity phoneCallEntity)
        {
            Device = phoneCallEntity is RemotePhoneCallEntity ? Resources.FromCloud : Resources.FromLocal;
            return this;
        }

        IActivityModelBuilder IActivityModelBuilder.WithDevice(SmsMessageEntity smsMessageEntity)
        {
            Device = smsMessageEntity is RemoteSmsMessageEntity ? Resources.FromCloud : Resources.FromLocal;
            return this;
        }

        public ActivityModel Build()
        {
            return this;
        }

        public static IActivityModelBuilder BeginBuild(IEntity entity)
        {
            return new ActivityModel(entity);
        }
    }

    public interface IActivityModelBuilder
    {
        IActivityModelBuilder WithContact(ContactEntity contactEntity);

        IActivityModelBuilder WithType(ActivityTypeEnum type);

        IActivityModelBuilder WithContent(String content);

        IActivityModelBuilder WithContent(UpdateEntity update);

        IActivityModelBuilder WithDevice(ClippingDto.ClippingSourceEnum clippingSourceEnum);

        IActivityModelBuilder WithDevice(PhoneCallEntity phoneCallEntity);

        IActivityModelBuilder WithDevice(SmsMessageEntity smsMessageEntity);

        ActivityModel Build();
    }
}