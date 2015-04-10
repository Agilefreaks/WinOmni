namespace OmnipasteTests.NotificationList
{
    using System.Reactive.Linq;
    using FluentAssertions;
    using Moq;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using Omnipaste.Framework.Models.Factories;
    using Omnipaste.NotificationList;
    using Omnipaste.NotificationList.Notification;
    using Omnipaste.NotificationList.Notification.ClippingNotification;
    using Omnipaste.NotificationList.Notification.IncomingCallNotification;
    using Omnipaste.NotificationList.Notification.IncomingSmsNotification;

    [TestFixture]
    public class NotificationViewModelFactoryTests
    {
        private NotificationViewModelFactory _subject;

        private MoqMockingKernel _kernel;

        private Mock<IConversationModelFactory> _mockConversationModelFactory;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _kernel.Bind<IIncomingCallNotificationViewModel>().To<IncomingCallNotificationViewModel>();
            _kernel.Bind<IIncomingSmsNotificationViewModel>().To<IncomingSmsNotificationViewModel>();
            _kernel.Bind<IClippingNotificationViewModel>().To<ClippingNotificationViewModel>();
            _mockConversationModelFactory = new Mock<IConversationModelFactory>();

            _subject = new NotificationViewModelFactory(_kernel, _mockConversationModelFactory.Object);
        }

        [Test]
        public void Create_WithClipping_SetsTheClippingOnTheViewModel()
        {
            var clipping = new ClippingEntity();
            var viewModel = (IClippingNotificationViewModel)_subject.Create(clipping).Wait();

            viewModel.Resource.Should().Be(clipping);
        }

        [Test]
        public void Create_WithCallNotification_SetsThePhoneNumberOnTheModel()
        {
            var contactModel = new ContactModel(new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "your number" } } });
            var remotePhoneCall = new RemotePhoneCallEntity();
            var observable = Observable.Return(new RemotePhoneCallModel(remotePhoneCall) { ContactModel = contactModel });
            _mockConversationModelFactory.Setup(m => m.Create<RemotePhoneCallModel, RemotePhoneCallEntity>(remotePhoneCall)).Returns(observable);
            var notificationViewModel = (IncomingCallNotificationViewModel)_subject.Create(remotePhoneCall).Wait();

            notificationViewModel.Line1.Should().Be("your number");
        }
        [Test]
        public void Create_WithCall_SetsTheEventOnTheViewModelAsTheResource()
        {
            var remotePhoneCall = new RemotePhoneCallEntity();
            var remotePhoneCallModel = new RemotePhoneCallModel(remotePhoneCall);
            var observable = Observable.Return(remotePhoneCallModel);
            _mockConversationModelFactory.Setup(m => m.Create<RemotePhoneCallModel, RemotePhoneCallEntity>(remotePhoneCall)).Returns(observable);
            var viewModel = (IConversationNotificationViewModel)_subject.Create(remotePhoneCall).Wait();

            viewModel.Resource.Should().Be(remotePhoneCallModel);
        }


        [Test]
        public void Create_WithMessageAndNoContactNamePresent_SetsThePhoneAndContentProperties()
        {
            var contactModel = new ContactModel(new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "1234567" } } });
            var remoteSmsMessage = new RemoteSmsMessageEntity { Content = "SmsContent" };
            var remoteSmsMessageModel = new RemoteSmsMessageModel(remoteSmsMessage) { ContactModel = contactModel };
            var observable = Observable.Return(remoteSmsMessageModel);
            _mockConversationModelFactory.Setup(m => m.Create<RemoteSmsMessageModel, RemoteSmsMessageEntity>(remoteSmsMessage)).Returns(observable);
            var notificationViewModel = (IIncomingSmsNotificationViewModel)_subject.Create(remoteSmsMessage).Wait();

            notificationViewModel.Line1.Should().Be("1234567");
            notificationViewModel.Line2.Should().Be("SmsContent");
        }

        [Test]
        public void Create_WithEventOfTypeIncomingSmsAndContactNamePresent_SetsTheContactNameAndContentProperties()
        {
            var contactModel = new ContactModel(new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "1234567" } }, FirstName = "Test", LastName = "Contact" });
            var remoteSmsMessage = new RemoteSmsMessageEntity { Content = "SmsContent" };
            var remoteSmsMessageModel = new RemoteSmsMessageModel(remoteSmsMessage) { ContactModel = contactModel };
            var observable = Observable.Return(remoteSmsMessageModel);
            _mockConversationModelFactory.Setup(m => m.Create<RemoteSmsMessageModel, RemoteSmsMessageEntity>(remoteSmsMessage)).Returns(observable);
            var notificationViewModel = (IIncomingSmsNotificationViewModel)_subject.Create(remoteSmsMessage).Wait();

            notificationViewModel.Line1.Should().Be("Test Contact");
            notificationViewModel.Line2.Should().Be("SmsContent");
        }
    }
}
