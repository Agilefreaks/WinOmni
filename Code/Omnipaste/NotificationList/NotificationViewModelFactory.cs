﻿namespace Omnipaste.NotificationList
{
    using System;
    using System.Collections.Generic;
    using Events.Models;
    using Ninject;
    using Omnipaste.Notification;
    using Clipboard.Models;
    using Omnipaste.Notification.ClippingNotification;
    using Omnipaste.Notification.HyperlinkNotification;
    using Omnipaste.Notification.IncomingCallNotification;
    using Omnipaste.Notification.IncomingSmsNotification;

    public class NotificationViewModelFactory : INotificationViewModelFactory
    {
        [Inject]
        public IKernel Kernel { get; set; }

        private readonly IDictionary<Clipping.ClippingTypeEnum, Func<INotificationViewModel>> _clippingNotificationConstructors;

        private readonly IDictionary<EventTypeEnum, Func<IEventNotificationViewModel>> _eventNotificationConstructors; 

        public NotificationViewModelFactory(IKernel kernel)
        {
            Kernel = kernel;

            _clippingNotificationConstructors = new Dictionary<Clipping.ClippingTypeEnum, Func<INotificationViewModel>>();
            _clippingNotificationConstructors.Add(Clipping.ClippingTypeEnum.Url, () => Kernel.Get<IHyperlinkNotificationViewModel>());
            _clippingNotificationConstructors.Add(Clipping.ClippingTypeEnum.Unknown, () => Kernel.Get<IClippingNotificationViewModel>());
            _clippingNotificationConstructors.Add(Clipping.ClippingTypeEnum.Address, () => Kernel.Get<IClippingNotificationViewModel>());
            _clippingNotificationConstructors.Add(Clipping.ClippingTypeEnum.PhoneNumber, () => Kernel.Get<IClippingNotificationViewModel>());

            _eventNotificationConstructors = new Dictionary<EventTypeEnum, Func<IEventNotificationViewModel>>();
            _eventNotificationConstructors.Add(EventTypeEnum.IncomingCallEvent, () => Kernel.Get<IIncomingCallNotificationViewModel>());
            _eventNotificationConstructors.Add(EventTypeEnum.IncomingSmsEvent, () => Kernel.Get<IIncomingSmsNotificationViewModel>());
        }

        public INotificationViewModel Create(Clipping clipping)
        {
            INotificationViewModel result = _clippingNotificationConstructors[clipping.Type]();
            result.Message = clipping.Content;

            return result;
        }

        public INotificationViewModel Create(Event @event)
        {
            IEventNotificationViewModel result = _eventNotificationConstructors[@event.Type]();
            result.PhoneNumber = @event.PhoneNumber;
            result.Message = @event.Content;

            return result;
        }
    }
}