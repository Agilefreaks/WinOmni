namespace OmnipasteTests.Notification.IncomingCallNotification
{
    using System.Windows.Threading;
    using Caliburn.Micro;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Models;
    using Omnipaste.Notification;
    using Omnipaste.Notification.IncomingCallNotification;
    using PhoneCalls.Resources.v1;

    [TestFixture]
    public class IncomingCallNotificationViewModelTests
    {
        private IncomingCallNotificationViewModel _subject;

        private MoqMockingKernel _kernel;

        private Mock<IPhoneCalls> _mockPhoneCalls;

        private Mock<IEventAggregator> _mockEventAggregator;

        private Mock<IApplicationService> _mockApplicationService;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _mockPhoneCalls = _kernel.GetMock<IPhoneCalls>();
            _mockPhoneCalls.DefaultValue = DefaultValue.Mock;
            
            _mockEventAggregator = _kernel.GetMock<IEventAggregator>();
            _kernel.Bind<IEventAggregator>().ToConstant(_mockEventAggregator.Object).InSingletonScope();

            _mockApplicationService = _kernel.GetMock<IApplicationService>();
            _mockApplicationService.Setup(s => s.Dispatcher).Returns(Dispatcher.CurrentDispatcher);
            _kernel.Bind<IApplicationService>().ToConstant(_mockApplicationService.Object);
            
            _subject = _kernel.Get<IncomingCallNotificationViewModel>();
            _subject.State = ViewModelStatusEnum.Open;
        }

        [Test]
        public void EndCall_EndsThePhoneCallCorespondingToTheAssociatedResource()
        {
            const string ResourceId = "someId";
            _subject.Resource = new Call { Id = ResourceId };
            
            _subject.EndCall();

            _mockPhoneCalls.Verify(p => p.EndCall(ResourceId), Times.Once);
        }

        [Test]
        public void ReplyWithSms_CallsPhonesSendSms()
        {
            _subject.Resource = new Call { ContactInfo = new ContactInfo { Phone = "1234567"  } };
            _subject.ReplyWithSMS();

            _mockEventAggregator.Verify(ea => ea.Publish(It.Is<SendSmsMessage>(m => m.Recipient == "1234567" && m.Message == ""), It.IsAny<System.Action<System.Action>>()));
        }
    }
}