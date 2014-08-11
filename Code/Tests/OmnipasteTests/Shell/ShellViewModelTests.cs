namespace OmnipasteTests.Shell
{
    using Caliburn.Micro;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.SendSms;
    using Omnipaste.Shell;

    [TestFixture]
    public class ShellViewModelTests
    {
        #region Fields

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

            _mockSessionManager = new Mock<ISessionManager> { DefaultValue = DefaultValue.Mock };
            kernel.Bind<ISessionManager>().ToConstant(_mockSessionManager.Object);

            _eventAggregator = new EventAggregator();
            kernel.Bind<IEventAggregator>().ToConstant(_eventAggregator).InSingletonScope();
            
            kernel.Bind<IShellViewModel>().To<ShellViewModel>();

            _subject = kernel.Get<ShellViewModel>();
        }        
    }
}