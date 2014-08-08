namespace Omnipaste.NotificationList
{
    using Events.Models;
    using Ninject;
    using Omnipaste.Notification;
    using Clipboard.Models;
    using Omnipaste.Notification.ClippingNotification;
    using Omnipaste.Notification.HyperlinkNotification;
    using Omnipaste.Notification.IncomingCallNotification;
    using Omnipaste.Notification.Models;

    public class NotificationViewModelFactory : INotificationViewModelFactory
    {
        [Inject]
        public IKernel Kernel { get; set; }

        public INotificationViewModel Create(Clipping clipping)
        {
            INotificationViewModel viewModel;

            if (clipping.Type == Clipping.ClippingTypeEnum.Url)
            {
                var model = new HyperlinkNotification { Title = "Incoming Link", Message = clipping.Content };
                var hyperlinkNotificationViewModel = Kernel.Get<IHyperlinkNotificationViewModel>();
                hyperlinkNotificationViewModel.Model = model;

                viewModel = hyperlinkNotificationViewModel;
            }
            else
            {
                var model = new ClippingNotification { Title = "New clipping", Message = clipping.Content };
                viewModel = new ClippingNotificationViewModel { Model = model };
            }

            return viewModel;
        }

        public INotificationViewModel Create(Event @event)
        {
            var model = new IncomingCallNotification
                        {
                            Title = string.Concat("Incoming call from ", @event.phone_number),
                            PhoneNumber = @event.phone_number
                        };

            var viewModel = Kernel.Get<IIncomingCallNotificationViewModel>();
            viewModel.Model = model;

            return viewModel;
        }
    }
}