namespace OmnipasteTests.NotificationList
{
    using System;
    using System.Reactive.Linq;
    using System.Windows.Navigation;
    using FluentAssertions;
    using Moq;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Models;
    using Omnipaste.Notification;
    using Omnipaste.Notification.ClippingNotification;
    using Omnipaste.Notification.IncomingCallNotification;
    using Omnipaste.Notification.IncomingSmsNotification;
    using Omnipaste.NotificationList;
    using Omnipaste.Presenters;
    using Omnipaste.Presenters.Factories;
    using OmnipasteTests.Helpers;

    [TestFixture]
    public class NotificationViewModelFactoryTests
    {
        private NotificationViewModelFactory _subject;

        private MoqMockingKernel _kernel;

        private Mock<IConversationPresenterFactory> _mockConversationPresenterFactory;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _kernel.Bind<IIncomingCallNotificationViewModel>().To<IncomingCallNotificationViewModel>();
            _kernel.Bind<IIncomingSmsNotificationViewModel>().To<IncomingSmsNotificationViewModel>();
            _kernel.Bind<IClippingNotificationViewModel>().To<ClippingNotificationViewModel>();
            _mockConversationPresenterFactory = new Mock<IConversationPresenterFactory>();

            _subject = new NotificationViewModelFactory(_kernel, _mockConversationPresenterFactory.Object);
        }

        [Test]
        public void Create_WithClipping_SetsTheClippingOnTheViewModel()
        {
            var clipping = new ClippingModel();
            var viewModel = (IClippingNotificationViewModel)_subject.Create(clipping).Wait();

            viewModel.Resource.Should().Be(clipping);
        }

        [Test]
        public void Create_WithCallNotification_SetsThePhoneNumberOnTheModel()
        {
            var contactInfoPresenter = new ContactInfoPresenter(new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "your number" } } });
            var remotePhoneCall = new RemotePhoneCall();
            var observable = Observable.Return(new RemotePhoneCallPresenter(remotePhoneCall) { ContactInfoPresenter = contactInfoPresenter });
            _mockConversationPresenterFactory.Setup(m => m.Create<RemotePhoneCallPresenter, RemotePhoneCall>(remotePhoneCall)).Returns(observable);
            var notificationViewModel = (IncomingCallNotificationViewModel)_subject.Create(remotePhoneCall).Wait();

            notificationViewModel.Line1.Should().Be("your number");
        }
        [Test]
        public void Create_WithCall_SetsTheEventOnTheViewModelAsTheResource()
        {
            var remotePhoneCall = new RemotePhoneCall();
            var remotePhoneCallPresenter = new RemotePhoneCallPresenter(remotePhoneCall);
            var observable = Observable.Return(remotePhoneCallPresenter);
            _mockConversationPresenterFactory.Setup(m => m.Create<RemotePhoneCallPresenter, RemotePhoneCall>(remotePhoneCall)).Returns(observable);
            var viewModel = (IConversationNotificationViewModel)_subject.Create(remotePhoneCall).Wait();

            viewModel.Resource.Should().Be(remotePhoneCallPresenter);
        }


        [Test]
        public void Create_WithMessageAndNoContactNamePresent_SetsThePhoneAndContentProperties()
        {
            var contactInfoPresenter = new ContactInfoPresenter(new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "1234567" } } });
            var remoteSmsMessage = new RemoteSmsMessage { Content = "SmsContent" };
            var remoteSmsMessagePresenter = new RemoteSmsMessagePresenter(remoteSmsMessage) { ContactInfoPresenter = contactInfoPresenter };
            var observable = Observable.Return(remoteSmsMessagePresenter);
            _mockConversationPresenterFactory.Setup(m => m.Create<RemoteSmsMessagePresenter, RemoteSmsMessage>(remoteSmsMessage)).Returns(observable);
            var notificationViewModel = (IIncomingSmsNotificationViewModel)_subject.Create(remoteSmsMessage).Wait();

            notificationViewModel.Line1.Should().Be("1234567");
            notificationViewModel.Line2.Should().Be("SmsContent");
        }

        [Test]
        public void Create_WithEventOfTypeIncomingSmsAndContactNamePresent_SetsTheContactNameAndContentProperties()
        {
            var contactInfoPresenter = new ContactInfoPresenter(new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "1234567" } }, FirstName = "Test", LastName = "Contact" });
            var remoteSmsMessage = new RemoteSmsMessage { Content = "SmsContent" };
            var remoteSmsMessagePresenter = new RemoteSmsMessagePresenter(remoteSmsMessage) { ContactInfoPresenter = contactInfoPresenter };
            var observable = Observable.Return(remoteSmsMessagePresenter);
            _mockConversationPresenterFactory.Setup(m => m.Create<RemoteSmsMessagePresenter, RemoteSmsMessage>(remoteSmsMessage)).Returns(observable);
            var notificationViewModel = (IIncomingSmsNotificationViewModel)_subject.Create(remoteSmsMessage).Wait();

            notificationViewModel.Line1.Should().Be("Test Contact");
            notificationViewModel.Line2.Should().Be("SmsContent");
        }
    }
}
