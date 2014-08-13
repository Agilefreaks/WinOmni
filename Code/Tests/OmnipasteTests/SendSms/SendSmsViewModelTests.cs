namespace OmnipasteTests.SendSms
{
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniApi.Resources.v1;
    using Omnipaste.SendSms;

    [TestFixture]
    public class SendSmsViewModelTests
    {
        private ISendSmsViewModel _subject;

        private MoqMockingKernel _kernel;

        private Mock<IPhones> _mockPhones;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _mockPhones = _kernel.GetMock<IPhones>();
            _mockPhones.DefaultValue = DefaultValue.Mock;
            _kernel.Bind<IPhones>().ToConstant(_mockPhones.Object);
            _subject = _kernel.Get<SendSmsViewModel>();
        }

        [Test]
        public void Send_CallsPhoneApiSendSms()
        {
            _subject.Model = new SmsMessage
                             {
                                 Message = "save me Obi-Wan Kenobi",
                                 Recipient = "1234567"
                             };

            _subject.Send();

            _mockPhones.Verify(p => p.SendSms("1234567", "save me Obi-Wan Kenobi"), Times.Once);
        }
    }
}