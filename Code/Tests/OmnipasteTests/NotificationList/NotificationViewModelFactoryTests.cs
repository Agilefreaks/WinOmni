namespace OmnipasteTests.NotificationList
{
    using System.Reactive.Linq;
    using FluentAssertions;
    using Moq;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Models.Factories;
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

        private Mock<IConversationModelFactory> _mockConversationPresenterFactory;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _kernel.Bind<IIncomingCallNotificationViewModel>().To<IncomingCallNotificationViewModel>();
            _kernel.Bind<IIncomingSmsNotificationViewModel>().To<IncomingSmsNotificationViewModel>();
            _kernel.Bind<IClippingNotificationViewModel>().To<ClippingNotificationViewModel>();
            _mockConversationPresenterFactory = new Mock<IConversationModelFactory>();

            _subject = new NotificationViewModelFactory(_kernel, _mockConversationPresenterFactory.Object);
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
            var contactInfoPresenter = new ContactModel(new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "your number" } } });
            var remotePhoneCall = new RemotePhoneCallEntity();
            var observable = Observable.Return(new RemotePhoneCallModel(remotePhoneCall) { ContactModel = contactInfoPresenter });
            _mockConversationPresenterFactory.Setup(m => m.Create<RemotePhoneCallModel, RemotePhoneCallEntity>(remotePhoneCall)).Returns(observable);
            var notificationViewModel = (IncomingCallNotificationViewModel)_subject.Create(remotePhoneCall).Wait();

            notificationViewModel.Line1.Should().Be("your number");
        }
        [Test]
        public void Create_WithCall_SetsTheEventOnTheViewModelAsTheResource()
        {
            var remotePhoneCall = new RemotePhoneCallEntity();
            var remotePhoneCallPresenter = new RemotePhoneCallModel(remotePhoneCall);
            var observable = Observable.Return(remotePhoneCallPresenter);
            _mockConversationPresenterFactory.Setup(m => m.Create<RemotePhoneCallModel, RemotePhoneCallEntity>(remotePhoneCall)).Returns(observable);
            var viewModel = (IConversationNotificationViewModel)_subject.Create(remotePhoneCall).Wait();

            viewModel.Resource.Should().Be(remotePhoneCallPresenter);
        }


        [Test]
        public void Create_WithMessageAndNoContactNamePresent_SetsThePhoneAndContentProperties()
        {
            var contactInfoPresenter = new ContactModel(new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "1234567" } } });
            var remoteSmsMessage = new RemoteSmsMessageEntity { Content = "SmsContent" };
            var remoteSmsMessagePresenter = new RemoteSmsMessageModel(remoteSmsMessage) { ContactModel = contactInfoPresenter };
            var observable = Observable.Return(remoteSmsMessagePresenter);
            _mockConversationPresenterFactory.Setup(m => m.Create<RemoteSmsMessageModel, RemoteSmsMessageEntity>(remoteSmsMessage)).Returns(observable);
            var notificationViewModel = (IIncomingSmsNotificationViewModel)_subject.Create(remoteSmsMessage).Wait();

            notificationViewModel.Line1.Should().Be("1234567");
            notificationViewModel.Line2.Should().Be("SmsContent");
        }

        [Test]
        public void Create_WithEventOfTypeIncomingSmsAndContactNamePresent_SetsTheContactNameAndContentProperties()
        {
            var contactInfoPresenter = new ContactModel(new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "1234567" } }, FirstName = "Test", LastName = "Contact" });
            var remoteSmsMessage = new RemoteSmsMessageEntity { Content = "SmsContent" };
            var remoteSmsMessagePresenter = new RemoteSmsMessageModel(remoteSmsMessage) { ContactModel = contactInfoPresenter };
            var observable = Observable.Return(remoteSmsMessagePresenter);
            _mockConversationPresenterFactory.Setup(m => m.Create<RemoteSmsMessageModel, RemoteSmsMessageEntity>(remoteSmsMessage)).Returns(observable);
            var notificationViewModel = (IIncomingSmsNotificationViewModel)_subject.Create(remoteSmsMessage).Wait();

            notificationViewModel.Line1.Should().Be("Test Contact");
            notificationViewModel.Line2.Should().Be("SmsContent");
        }
    }
}
