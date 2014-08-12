namespace OmnipasteTests.NotificationList
{
    using Events.Models;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Notification.IncomingCallNotification;
    using Omnipaste.NotificationList;

    [TestFixture]
    public class NotificationViewModelFactoryTests
    {
        private NotificationViewModelFactory _subject;

        [Test]
        public void Create_WithIncomingCallNotification_SetsThePhoneNumberOnTheModel()
        {
            var kernel = new MoqMockingKernel();
            var mockIncomingCallNotificationViewModel = kernel.GetMock<IIncomingCallNotificationViewModel>();
            kernel.Bind<IIncomingCallNotificationViewModel>().ToConstant(mockIncomingCallNotificationViewModel.Object);
            _subject = new NotificationViewModelFactory { Kernel = kernel };

            var notificationViewModel = (IIncomingCallNotificationViewModel)_subject.Create(new Event { phone_number = "your number", Type = EventTypeEnum.IncomingCallEvent});

            mockIncomingCallNotificationViewModel.VerifySet(vm => vm.PhoneNumber = "your number");
        }
    }
}