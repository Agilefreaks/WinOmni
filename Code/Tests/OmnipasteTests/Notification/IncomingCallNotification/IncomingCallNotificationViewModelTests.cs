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
    using Omnipaste.Notification.Models;

    [TestFixture]
    public class IncomingCallNotificationViewModelTests
    {
        private IncomingCallNotificationViewModel _subject;

        private MoqMockingKernel _kernel;

        private Mock<IPhones> _mockPhones;

        private Mock<IEventAggregator> _mockEventAggregator;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _mockPhones = _kernel.GetMock<IPhones>();
            _mockPhones.DefaultValue = DefaultValue.Mock;
            
            _mockEventAggregator = _kernel.GetMock<IEventAggregator>();
            _kernel.Bind<IEventAggregator>().ToConstant(_mockEventAggregator.Object).InSingletonScope();
            
            _subject = _kernel.Get<IncomingCallNotificationViewModel>();
        }

        [Test]
        public void EndCall_CallsPhonesEndCallMethod()
        {
            _subject.EndCall();

            _mockPhones.Verify(p => p.EndCall(), Times.Once);
        }

        [Test]
        public void ReplyWithSms_CallsPhonesSendSms()
        {
            _subject.Model = new IncomingCallNotification { PhoneNumber = "1234567" };
            _subject.ReplyWithSms();

            _mockEventAggregator.Verify(ea => ea.Publish(It.Is<SendSmsMessage>(m => m.Recipient == "1234567" && m.Message == ""), It.IsAny<System.Action<System.Action>>()));
        }
    }
}