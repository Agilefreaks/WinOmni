namespace OmnipasteTests.NotificationList
{
    using FluentAssertions;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Models;
    using Omnipaste.Notification;
    using Omnipaste.Notification.ClippingNotification;
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
            _kernel.Bind<IClippingNotificationViewModel>().To<ClippingNotificationViewModel>();

            _subject = new NotificationViewModelFactory(_kernel);
        }

        [Test]
        public void Create_WithClipping_SetsTheClippingOnTheViewModel()
        {
            var clipping = new ClippingModel();
            var viewModel = (IClippingNotificationViewModel)_subject.Create(clipping);

            viewModel.Resource.Should().Be(clipping);
        }

        [Test]
        public void Create_WithCallNotification_SetsThePhoneNumberOnTheModel()
        {
            var notificationViewModel = (IncomingCallNotificationViewModel)_subject.Create(new Call { ContactInfo = new ContactInfo { Phone = "your number" } });

            notificationViewModel.Line1.Should().Be("your number");
        }

        [Test]
        public void Create_WithCall_SetsTheEventOnTheViewModelAsTheResource()
        {
            var call = new Call();
            var viewModel = (IConversationNotificationViewModel)_subject.Create(call);

            viewModel.Resource.Should().Be(call);
        }

        [Test]
        public void Create_WithMessageAndNoContactNamePresent_SetsThePhoneAndContentProperties()
        {
            var notificationViewModel = (IIncomingSmsNotificationViewModel)_subject.Create(
                new Message { ContactInfo = new ContactInfo { Phone = "1234567" }, Content = "SmsContent" });

            notificationViewModel.Line1.Should().Be("1234567");
            notificationViewModel.Line2.Should().Be("SmsContent");
        }

        [Test]
        public void Create_WithEventOfTypeIncomingSmsAndContactNamePresent_SetsTheContactNameAndContentProperties()
        {
            var notificationViewModel = (IIncomingSmsNotificationViewModel)_subject.Create(
                new Message { ContactInfo = new ContactInfo { Phone = "1234567", FirstName = "Test", LastName = "Contact" }, Content = "SmsContent" });

            notificationViewModel.Line1.Should().Be("Test Contact");
            notificationViewModel.Line2.Should().Be("SmsContent");
        }
    }
}