namespace ClipboardTests.Handlers
{
    using Clipboard.API;
    using Clipboard.Handlers;
    using Clipboard.Handlers.WindowsClipboard;
    using Clipboard.Models;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel;
    using NUnit.Framework;

    [TestFixture]
    public class LocalClipboardHandlerTests
    {
        private MockingKernel _mockingKernel;

        private ILocalClipboardHandler _subject;

        private Mock<IWindowsClipboardWrapper> _mockWindowsClipboardWrapper;

        private Mock<IClippingsApi> _mockClippingsApi;

        [SetUp]
        public void SetUp()
        {
            _mockingKernel = new MockingKernel();

            _mockWindowsClipboardWrapper = new Mock<IWindowsClipboardWrapper>();
            _mockClippingsApi = new Mock<IClippingsApi>();

            _mockingKernel.Bind<IWindowsClipboardWrapper>().ToConstant(_mockWindowsClipboardWrapper.Object);
            _mockingKernel.Bind<IClippingsApi>().ToConstant(_mockClippingsApi.Object);

            _mockingKernel.Bind<ILocalClipboardHandler>().To<LocalClipboardsHandler>();

            _subject = _mockingKernel.Get<ILocalClipboardHandler>();
        }

        [Test]
        public void Subscribe_Always_StartsWatchingTheClipboardWrapper()
        {
            _subject.Subscribe(new Mock<System.IObserver<Clipping>>().Object);

            _mockWindowsClipboardWrapper.Verify(wc => wc.StartWatchingClipboard(), Times.Once);
        }

        [Test]
        public void PostClipping_Always_CallsPostClipping()
        {
            _subject.PostClipping(new Clipping("some stuff"));

            _mockWindowsClipboardWrapper.Verify(wc => wc.SetData("some stuff"), Times.Once);
        }
    }
}