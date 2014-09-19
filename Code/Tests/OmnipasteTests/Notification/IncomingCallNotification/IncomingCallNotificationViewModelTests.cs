namespace OmnipasteTests.Notification.IncomingCallNotification
{
    using System.Windows.Threading;
    using Caliburn.Micro;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Framework;
    using Omnipaste.Notification;
    using Omnipaste.Notification.IncomingCallNotification;

    [TestFixture]
    public class IncomingCallNotificationViewModelTests
    {
        private IncomingCallNotificationViewModel _subject;

        private MoqMockingKernel _kernel;

        private Mock<IDevices> _mockDevices;

        private Mock<IEventAggregator> _mockEventAggregator;

        private Mock<IApplicationService> _mockApplicationService;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _mockDevices = _kernel.GetMock<IDevices>();
            _mockDevices.DefaultValue = DefaultValue.Mock;
            
            _mockEventAggregator = _kernel.GetMock<IEventAggregator>();
            _kernel.Bind<IEventAggregator>().ToConstant(_mockEventAggregator.Object).InSingletonScope();

            _mockApplicationService = _kernel.GetMock<IApplicationService>();
            _mockApplicationService.Setup(s => s.Dispatcher).Returns(Dispatcher.CurrentDispatcher);
            _kernel.Bind<IApplicationService>().ToConstant(_mockApplicationService.Object);
            
            _subject = _kernel.Get<IncomingCallNotificationViewModel>();
            _subject.State = ViewModelStatusEnum.Open;
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