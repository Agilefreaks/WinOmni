namespace Omnipaste.NotificationList
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Clipboard.Dto;
    using Ninject;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using Omnipaste.Framework.Models.Factories;
    using Omnipaste.NotificationList.Notification;
    using Omnipaste.NotificationList.Notification.ClippingNotification;
    using Omnipaste.NotificationList.Notification.HyperlinkNotification;
    using Omnipaste.NotificationList.Notification.IncomingCallNotification;
    using Omnipaste.NotificationList.Notification.IncomingSmsNotification;

    public class NotificationViewModelFactory : INotificationViewModelFactory
    {
        private readonly IConversationModelFactory _conversationModelFactory;

        public IKernel Kernel { get; set; }

        private readonly IDictionary<ClippingDto.ClippingTypeEnum, Func<IClippingNotificationViewModel>> _clippingNotificationConstructors;

        public NotificationViewModelFactory(IKernel kernel, IConversationModelFactory conversationModelFactory)
        {
            Kernel = kernel;
            
            _conversationModelFactory = conversationModelFactory;
            _clippingNotificationConstructors = new Dictionary<ClippingDto.ClippingTypeEnum, Func<IClippingNotificationViewModel>>()
                                                    {
                                                        { ClippingDto.ClippingTypeEnum.Url, () => Kernel.Get<IHyperlinkNotificationViewModel>() },
                                                        { ClippingDto.ClippingTypeEnum.Unknown, () => Kernel.Get<IClippingNotificationViewModel>() },
                                                        { ClippingDto.ClippingTypeEnum.Address, () => Kernel.Get<IClippingNotificationViewModel>() },
                                                        { ClippingDto.ClippingTypeEnum.PhoneNumber, () => Kernel.Get<IClippingNotificationViewModel>() }
                                                    };
        }

        public IObservable<INotificationViewModel> Create(ClippingEntity clipping)
        {
            var result = _clippingNotificationConstructors[clipping.Type]();
            result.Resource = clipping;

            return Observable.Return(result);
        }

        public IObservable<INotificationViewModel> Create(RemotePhoneCallEntity phoneCallEntity)
        {
            var result = Kernel.Get<IIncomingCallNotificationViewModel>();

            return _conversationModelFactory.Create<RemotePhoneCallModel, RemotePhoneCallEntity>(phoneCallEntity).Select(
                p =>
                    {
                        result.Resource = p;
                        return result;
                    });;
        }

        public IObservable<INotificationViewModel> Create(RemoteSmsMessageEntity smsMessageEntity)
        {
            var result = Kernel.Get<IIncomingSmsNotificationViewModel>();

            return _conversationModelFactory.Create<RemoteSmsMessageModel, RemoteSmsMessageEntity>(smsMessageEntity).Select(
                m =>
                    {
                        result.Resource = m;
                        return result;
                    });
        }
    }
}