namespace OmnipasteTests.Shell
{
    using System.Reactive.Concurrency;
    using Caliburn.Micro;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Framework.Services;
    using Omnipaste.Shell;
    using Omnipaste.Shell.ContextMenu;
    using OmniUI.Framework.Services;

    [TestFixture]
    public class ShellViewModelTests
    {
        private ShellViewModel _subject;

        private Mock<IEventAggregator> _mockEventAggregator;

        private Mock<ISessionManager> _mockSessionManager;

        private Mock<IUiRefreshService> _mockUiRefreshService;

        [SetUp]
        public void Setup()
        {
            _mockEventAggregator = new Mock<IEventAggregator>();
            _mockSessionManager = new Mock<ISessionManager> { DefaultValue = DefaultValue.Mock };
            _mockUiRefreshService = new Mock<IUiRefreshService>();
            SchedulerProvider.Dispatcher = new NewThreadScheduler();
            _subject = new ShellViewModel(_mockEventAggregator.Object, _mockSessionManager.Object)
                           {
                               UiRefreshService = _mockUiRefreshService.Object,
                               ContextMenuViewModel = new Mock<IContextMenuViewModel>().Object
                           };
        }

        [Test]
        public void Start_Always_StartsTheUiRefreshService()
        {
            _subject.Show();

            _mockUiRefreshService.Verify(x => x.Start(), Times.Once);
        }

        [Test]
        public void Close_Always_StopsTheUiRefreshService()
        {
            _subject.Close();

            _mockUiRefreshService.Verify(x => x.Stop(), Times.Once);
        }
    }
}