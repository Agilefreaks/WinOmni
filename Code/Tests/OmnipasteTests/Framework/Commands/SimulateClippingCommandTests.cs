namespace OmnipasteTests.Framework.Commands
{
    using Clipboard.Handlers.WindowsClipboard;
    using Microsoft.Practices.ServiceLocation;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Framework.Commands;

    [TestFixture]
    public class SimulateClippingCommandTests
    {
        private SimulateClippingCommand _subject;

        private Mock<IServiceLocator> _mockServiceLocator;

        private ServiceLocatorProvider _initialServiceLocatorProvider;

        private Mock<IWindowsClipboardWrapper> _mockWindowsClipboardWrapper;

        [SetUp]
        public void Setup()
        {
            CacheCurrentServiceLocator();
            _mockServiceLocator = new Mock<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => _mockServiceLocator.Object);
            _mockWindowsClipboardWrapper = new Mock<IWindowsClipboardWrapper>();
            _mockServiceLocator.Setup(x => x.GetInstance<IWindowsClipboardWrapper>())
                .Returns(_mockWindowsClipboardWrapper.Object);
            _subject = new SimulateClippingCommand();
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.SetLocatorProvider(_initialServiceLocatorProvider);
        }

        [Test]
        public void Execute_Always_SetsTheStringFormOfTheParameterOnTheLocalClipboard()
        {
            const string Content = "some content";

            _subject.Execute(Content);

            _mockWindowsClipboardWrapper.Verify(x => x.SetData(Content), Times.Once());
        }

        private void CacheCurrentServiceLocator()
        {
            if (ServiceLocator.IsLocationProviderSet)
            {
                var serviceLocator = ServiceLocator.Current;
                _initialServiceLocatorProvider = () => serviceLocator;
            }
            else
            {
                _initialServiceLocatorProvider = null;
            }
        }
    }
}