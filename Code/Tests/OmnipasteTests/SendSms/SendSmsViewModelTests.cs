namespace OmnipasteTests.SendSms
{
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniApi.Resources.v1;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.SendSms;

    [TestFixture]
    public class SendSmsViewModelTests
    {
        private ISendSmsViewModel _subject;

        private MoqMockingKernel _kernel;

        private Mock<IDevices> _mockDevices;

        private EventAggregator _eventAggregator;

        private Mock<IDialogViewModel> _mockDialogViewModel;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _mockDevices = _kernel.GetMock<IDevices>();
            _mockDevices.DefaultValue = DefaultValue.Mock;

            _eventAggregator = new EventAggregator();
            _kernel.Bind<IEventAggregator>().ToConstant(_eventAggregator).InSingletonScope();

            _mockDialogViewModel = new Mock<IDialogViewModel>();
            _kernel.Bind<IDialogViewModel>().ToConstant(_mockDialogViewModel.Object).InSingletonScope();

            _kernel.Bind<IDevices>().ToConstant(_mockDevices.Object);
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

            _mockDevices.Verify(p => p.SendSms("1234567", "save me Obi-Wan Kenobi"), Times.Once);
        }

        [Test]
        public void ListensToSendSmsMessages()
        {
            _eventAggregator.PublishOnCurrentThread(new SendSmsMessage());

            _mockDialogViewModel.Verify(vm => vm.ActivateItem(It.IsAny<ISendSmsViewModel>()));
        }

        [Test]
        public void Handle_SendSms_WillSetPropertiesOnTheViewModel()
        {
            _eventAggregator.PublishOnCurrentThread(new SendSmsMessage { Recipient = "1234567", Message = "save me Obi Wan Kenobi" });

            _subject.Model.Recipient.Should().Be("1234567");
            _subject.Model.Message.Should().Be("save me Obi Wan Kenobi");
        }
    }
}