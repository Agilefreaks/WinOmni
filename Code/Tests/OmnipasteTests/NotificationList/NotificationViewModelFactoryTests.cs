namespace OmnipasteTests.NotificationList
{
    using Events.Models;
    using FluentAssertions;
    using Moq;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Notification.IncomingCallNotification;
    using Omnipaste.Notification.IncomingSmsNotification;
    using Omnipaste.NotificationList;

    [TestFixture]
    public class NotificationViewModelFactoryTests
    {
        private NotificationViewModelFactory _subject;

        private MoqMockingKernel _kernel;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _kernel.Bind<IIncomingCallNotificationViewModel>().To<IncomingCallNotificationViewModel>();
            _kernel.Bind<IIncomingSmsNotificationViewModel>().To<IncomingSmsNotificationViewModel>();

            _subject = new NotificationViewModelFactory(_kernel);
        }

        [Test]
        public void Create_WithIncomingCallNotification_SetsThePhoneNumberOnTheModel()
        {
            var notificationViewModel = (IncomingCallNotificationViewModel)_subject.Create(new Event { PhoneNumber = "your number", Type = EventTypeEnum.IncomingCallEvent});

            notificationViewModel.PhoneNumber.Should().Be("your number");
        }

        [Test]
        public void Create_WithEventOfTypeIncomingSms_SetsThePhoneAndContentProperties()
        {
            var notificationViewModel = (IIncomingSmsNotificationViewModel)_subject.Create(
                new Event { PhoneNumber = "1234567", Content = "SmsContent", Type = EventTypeEnum.IncomingSmsEvent });

            notificationViewModel.PhoneNumber.Should().Be("1234567");
            notificationViewModel.Message.Should().Be("SmsContent");
        }
    }
}