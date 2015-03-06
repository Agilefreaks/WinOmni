namespace Omnipaste.NotificationList
{
    using System;
    using System.Collections.Generic;
    using Ninject;
    using Omnipaste.Notification;
    using Clipboard.Models;
    using Omnipaste.Models;
    using Omnipaste.Notification.ClippingNotification;
    using Omnipaste.Notification.HyperlinkNotification;
    using Omnipaste.Notification.IncomingCallNotification;
    using Omnipaste.Notification.IncomingSmsNotification;

    public class NotificationViewModelFactory : INotificationViewModelFactory
    {
        [Inject]
        public IKernel Kernel { get; set; }

        private readonly IDictionary<Clipping.ClippingTypeEnum, Func<IClippingNotificationViewModel>> _clippingNotificationConstructors;

        public NotificationViewModelFactory(IKernel kernel)
        {
            Kernel = kernel;

            _clippingNotificationConstructors = new Dictionary<Clipping.ClippingTypeEnum, Func<IClippingNotificationViewModel>>();
            _clippingNotificationConstructors.Add(Clipping.ClippingTypeEnum.Url, () => Kernel.Get<IHyperlinkNotificationViewModel>());
            _clippingNotificationConstructors.Add(Clipping.ClippingTypeEnum.Unknown, () => Kernel.Get<IClippingNotificationViewModel>());
            _clippingNotificationConstructors.Add(Clipping.ClippingTypeEnum.Address, () => Kernel.Get<IClippingNotificationViewModel>());
            _clippingNotificationConstructors.Add(Clipping.ClippingTypeEnum.PhoneNumber, () => Kernel.Get<IClippingNotificationViewModel>());
        }

        public INotificationViewModel Create(ClippingModel clipping)
        {
            var result = _clippingNotificationConstructors[clipping.Type]();
            result.Resource = clipping;

            return result;
        }

        public INotificationViewModel Create(Call call)
        {
            var result = Kernel.Get<IIncomingCallNotificationViewModel>();
            result.Resource = call;

            return result;
        }

        public INotificationViewModel Create(SmsMessage smsMessage)
        {
            var result = Kernel.Get<IIncomingSmsNotificationViewModel>();
            result.Resource = smsMessage;

            return result;
        }
    }
}