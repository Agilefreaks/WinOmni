namespace Omnipaste.NotificationList
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Clipboard.Models;
    using Ninject;
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

        private readonly IDictionary<Clipping.ClippingTypeEnum, Func<IClippingNotificationViewModel>> _clippingNotificationConstructors;

        public NotificationViewModelFactory(IKernel kernel, IConversationPresenterFactory conversationPresenterFactory)
        {
            Kernel = kernel;
            
            _conversationPresenterFactory = conversationPresenterFactory;
            _clippingNotificationConstructors = new Dictionary<Clipping.ClippingTypeEnum, Func<IClippingNotificationViewModel>>()
                                                    {
                                                        { Clipping.ClippingTypeEnum.Url, () => Kernel.Get<IHyperlinkNotificationViewModel>() },
                                                        { Clipping.ClippingTypeEnum.Unknown, () => Kernel.Get<IClippingNotificationViewModel>() },
                                                        { Clipping.ClippingTypeEnum.Address, () => Kernel.Get<IClippingNotificationViewModel>() },
                                                        { Clipping.ClippingTypeEnum.PhoneNumber, () => Kernel.Get<IClippingNotificationViewModel>() }
                                                    };
        }

        public IObservable<INotificationViewModel> Create(ClippingModel clipping)
        {
            var result = _clippingNotificationConstructors[clipping.Type]();
            result.Resource = clipping;

            return Observable.Return(result);
        }

        public IObservable<INotificationViewModel> Create(RemotePhoneCall phoneCall)
        {
            var result = Kernel.Get<IIncomingCallNotificationViewModel>();

            return _conversationPresenterFactory.Create<RemotePhoneCallPresenter, RemotePhoneCall>(phoneCall).Select(
                p =>
                    {
                        result.Resource = p;
                        return result;
                    });;
        }

        public IObservable<INotificationViewModel> Create(RemoteSmsMessage smsMessage)
        {
            var result = Kernel.Get<IIncomingSmsNotificationViewModel>();

            return _conversationPresenterFactory.Create<RemoteSmsMessagePresenter, RemoteSmsMessage>(smsMessage).Select(
                m =>
                    {
                        result.Resource = m;
                        return result;
                    });
        }
    }
}