namespace OmnipasteTests.NotificationList
{
    using Events.Models;
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

        private Mock<IIncomingCallNotificationViewModel> _mockIncomingCallNotificationViewModel;

        private Mock<IIncomingSmsNotificationViewModel> _mockIncomingSmsNotificationVIewModel;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _mockIncomingCallNotificationViewModel = _kernel.GetMock<IIncomingCallNotificationViewModel>();
            _kernel.Bind<IIncomingCallNotificationViewModel>().ToConstant(_mockIncomingCallNotificationViewModel.Object);
            _mockIncomingSmsNotificationVIewModel = _kernel.GetMock<IIncomingSmsNotificationViewModel>();
            _kernel.Bind<IIncomingSmsNotificationViewModel>().ToConstant(_mockIncomingSmsNotificationVIewModel.Object);

            _subject = new NotificationViewModelFactory { Kernel = _kernel };
        }

        [Test]
        public void Create_WithIncomingCallNotification_SetsThePhoneNumberOnTheModel()
        {
            _subject.Create(new Event { PhoneNumber = "your number", Type = EventTypeEnum.IncomingCallEvent});

            _mockIncomingCallNotificationViewModel.VerifySet(vm => vm.PhoneNumber = "your number");
        }

        [Test]
        public void Create_WithEventOfTypeIncomingSms_SetsThePhoneAndContentProperties()
        {
            _subject.Create(
                new Event { PhoneNumber = "1234567", Content = "SmsContent", Type = EventTypeEnum.IncomingSmsEvent });

            _mockIncomingSmsNotificationVIewModel.VerifySet(vm => vm.PhoneNumber = "1234567");
            _mockIncomingSmsNotificationVIewModel.VerifySet(vm => vm.Message = "SmsContent");
        }
    }
}