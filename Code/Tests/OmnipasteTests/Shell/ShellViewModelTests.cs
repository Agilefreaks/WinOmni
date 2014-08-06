namespace OmnipasteTests.Shell
{
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.SendSms;
    using Omnipaste.Shell;
    using Omnipaste.Shell.ContextMenu;

    [TestFixture]
    public class ShellViewModelTests
    {
        #region Fields

        private Mock<IContextMenuViewModel> _mockContextViewModel;

        private ShellViewModel _subject;

        private Mock<ISessionManager> _mockSessionManager;

        private EventAggregator _eventAggregator;

        private Mock<IDialogViewModel> _mockDialogViewModel;

        private Mock<ISendSmsViewModel> _mockSendSmsViewModel;

        #endregion

        [SetUp]
        public void SetUp()
        {
            var kernel = new MoqMockingKernel();

            _mockContextViewModel = kernel.GetMock<IContextMenuViewModel>();
            
            _mockSessionManager = new Mock<ISessionManager> { DefaultValue = DefaultValue.Mock };
            kernel.Bind<ISessionManager>().ToConstant(_mockSessionManager.Object);

            _eventAggregator = new EventAggregator();
            kernel.Bind<IEventAggregator>().ToConstant(_eventAggregator).InSingletonScope();

            _mockDialogViewModel = new Mock<IDialogViewModel>();
            kernel.Bind<IDialogViewModel>().ToConstant(_mockDialogViewModel.Object).InSingletonScope();

            _mockSendSmsViewModel = new Mock<ISendSmsViewModel>();
            kernel.Bind<ISendSmsViewModel>().ToConstant(_mockSendSmsViewModel.Object);
            
            kernel.Bind<IShellViewModel>().To<ShellViewModel>();

            _subject = kernel.Get<ShellViewModel>();
        }

        [Test]
        public void ShellViewModel_ListensToSendSmsMessages()
        {
            _eventAggregator.PublishOnCurrentThread(new SendSmsMessage());

            _mockDialogViewModel.Verify(vm => vm.ActivateItem(It.IsAny<ISendSmsViewModel>()));
        }

        [Test]
        public void Handle_SendSms_WillSetPropertiesOnTheViewModel()
        {
            _eventAggregator.PublishOnCurrentThread(new SendSmsMessage { Recipient = "1234567", Message = "save me obi wan kenobi"});

            _mockSendSmsViewModel.VerifySet(vm => vm.Recipient = "1234567");
            _mockSendSmsViewModel.VerifySet(vm => vm.Message = "save me obi wan kenobi");
        }
    }
}