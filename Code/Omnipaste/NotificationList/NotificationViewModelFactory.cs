namespace Omnipaste.NotificationList
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Clipboard.Dto;
    using Ninject;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Notification;
    using Omnipaste.Notification.ClippingNotification;
    using Omnipaste.Notification.HyperlinkNotification;
    using Omnipaste.Notification.IncomingCallNotification;
    using Omnipaste.Notification.IncomingSmsNotification;
    using Omnipaste.Presenters;
    using Omnipaste.Presenters.Factories;

    public class NotificationViewModelFactory : INotificationViewModelFactory
    {
        private readonly IConversationPresenterFactory _conversationPresenterFactory;

        public IKernel Kernel { get; set; }

        private readonly IDictionary<ClippingDto.ClippingTypeEnum, Func<IClippingNotificationViewModel>> _clippingNotificationConstructors;

        public NotificationViewModelFactory(IKernel kernel, IConversationPresenterFactory conversationPresenterFactory)
        {
            Kernel = kernel;
            
            _conversationPresenterFactory = conversationPresenterFactory;
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

            return _conversationPresenterFactory.Create<RemotePhoneCallPresenter, RemotePhoneCallEntity>(phoneCallEntity).Select(
                p =>
                    {
                        result.Resource = p;
                        return result;
                    });;
        }

        public IObservable<INotificationViewModel> Create(RemoteSmsMessageEntity smsMessageEntity)
        {
            var result = Kernel.Get<IIncomingSmsNotificationViewModel>();

            return _conversationPresenterFactory.Create<RemoteSmsMessagePresenter, RemoteSmsMessageEntity>(smsMessageEntity).Select(
                m =>
                    {
                        result.Resource = m;
                        return result;
                    });
        }
    }
}