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
            _subject.Send();

            _mockPhones.Verify(p => p.SendSms(_subject.Recipient, _subject.Message), Times.Once);
        }
    }
}