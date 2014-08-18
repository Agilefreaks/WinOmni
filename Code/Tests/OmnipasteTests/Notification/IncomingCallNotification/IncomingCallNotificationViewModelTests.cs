namespace OmnipasteTests.Notification.IncomingCallNotification
{
    using Caliburn.Micro;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniApi.Resources.v1;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Notification.IncomingCallNotification;

    [TestFixture]
    public class IncomingCallNotificationViewModelTests
    {
        private IncomingCallNotificationViewModel _subject;

        private MoqMockingKernel _kernel;

        private Mock<IDevices> _mockDevices;

        private Mock<IEventAggregator> _mockEventAggregator;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _mockDevices = _kernel.GetMock<IDevices>();
            _mockDevices.DefaultValue = DefaultValue.Mock;
            
            _mockEventAggregator = _kernel.GetMock<IEventAggregator>();
            _kernel.Bind<IEventAggregator>().ToConstant(_mockEventAggregator.Object).InSingletonScope();
            
            _subject = _kernel.Get<IncomingCallNotificationViewModel>();
        }

        [Test]
        public void EndCall_CallsPhonesEndCallMethod()
        {
            _subject.EndCall();

            _mockDevices.Verify(p => p.EndCall(), Times.Once);
        }

        [Test]
        public void ReplyWithSms_CallsPhonesSendSms()
        {
            _subject.PhoneNumber = "1234567";
            _subject.ReplyWithSms();

            _mockEventAggregator.Verify(ea => ea.Publish(It.Is<SendSmsMessage>(m => m.Recipient == "1234567" && m.Message == ""), It.IsAny<System.Action<System.Action>>()));
        }
    }
}