namespace Omnipaste.Framework.Models
{
    using System;
    using Clipboard.Dto;
    using Omnipaste.Activities.ActivityList.Activity;
    using Omnipaste.Framework.Entities;

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