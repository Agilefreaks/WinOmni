namespace OmnipasteTests.SmsComposer
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
    using Omnipaste.Models;
    using Omnipaste.SmsComposer;

    [TestFixture]
    public class SmsComposerViewModelTests
    {
        private ISmsComposerViewModel _subject;

        private MoqMockingKernel _kernel;

        private Mock<IDevices> _mockDevices;

        private EventAggregator _eventAggregator;

        private Mock<IDialogViewModel> _mockDialogViewModel;

        private Mock<ISMSFactory> _mockSMSFactory;

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

            _mockSMSFactory = new Mock<ISMSFactory> { DefaultValue = DefaultValue.Mock };
            _kernel.Bind<ISMSFactory>().ToConstant(_mockSMSFactory.Object);

            _subject = _kernel.Get<SmsComposerViewModel>();
            _subject.Model = new SMSMessage { Message = "save me Obi-Wan Kenobi", Recipient = "1234567" };
        }

        [Test]
        public void Send_CallsPhoneApiSendSms()
        {
            _subject.Send();

            _mockDevices.Verify(p => p.SendSms("1234567", "save me Obi-Wan Kenobi"), Times.Once);
        }

        [Test]
        public void ListensToSendSmsMessages()
        {
            _eventAggregator.PublishOnCurrentThread(new SendSmsMessage());

            _mockDialogViewModel.Verify(vm => vm.ActivateItem(It.IsAny<ISmsComposerViewModel>()));
        }

        [Test]
        public void Handle_SendSms_WillCreateASMSInstanceWithTheReceivedMessage()
        {
            var message = new SendSmsMessage { Recipient = "1234567", Message = "save me Obi Wan Kenobi" };

            _eventAggregator.PublishOnCurrentThread(message);

            _mockSMSFactory.Verify(x => x.Create(message), Times.Once());
        }

        [Test]
        public void Handle_SendSms_WillSetTheStateToComposing()
        {
            _eventAggregator.PublishOnCurrentThread(new SendSmsMessage { Recipient = "1234567", Message = "save me Obi Wan Kenobi" });

            _subject.State.Should().Be(SmsComposerStatusEnum.Composing);
        }

        [Test]
        public void CanSend_WhenStatusIsComposingAndThereIsARecipientAndThereIsAMessage_IsTrue()
        {
            _subject.State = SmsComposerStatusEnum.Composing;

            _subject.CanSend.Should().Be(true);
        }

        [Test]
        public void CanSend_WhenStatusIsSending_IsFalse()
        {
            _subject.State = SmsComposerStatusEnum.Sending;

            _subject.CanSend.Should().Be(false);
        }

        [Test]
        public void CanSend_WhenStatusIsSent_IsFalse()
        {
            _subject.State = SmsComposerStatusEnum.Sent;

            _subject.CanSend.Should().Be(false);
        }

        [Test]
        public void CanSend_WhenBodyIsEmpty_IsFalse()
        {
            _subject.State = SmsComposerStatusEnum.Composing;

            _subject.Model.Message = string.Empty;
            
            _subject.CanSend.Should().Be(false);
        }

        [Test]
        public void CanSend_WhenRecipientIsEmpty_IsFalse()
        {
            _subject.State = SmsComposerStatusEnum.Composing;

            _subject.Model.Recipient = string.Empty;
            
            _subject.CanSend.Should().Be(false);
        }

        [Test]
        public void Send_Always_SetsTheStateToSending()
        {
            _subject.Send();

            _subject.State.Should().Be(SmsComposerStatusEnum.Sending);
        }

        [Test]
        public void WhenModelsPropertyChangesCanSendIsNotified()
        {
            bool called = false;
            
            _subject.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "CanSend")
                {
                    called = true;
                }
            };

            _subject.Model.Message = "new message";

            called.Should().BeTrue();
        }
    }
}