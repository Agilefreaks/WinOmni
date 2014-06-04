namespace ClipboardTests.Handlers
{
    using System;
    using Clipboard.Handlers;
    using Clipboard.Models;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel;
    using NUnit.Framework;

    [TestFixture]
    public class ClipboardHandlerTests
    {
        private MockingKernel _mockingKernel;

        private Mock<ILocalClipboardHandler> _mockLocalClipboardHandler;

        private Mock<IOmniClipboardHandler> _mockOmniClipboardHandler;

        private IClipboadHandler _subject;

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MockingKernel();

            _mockLocalClipboardHandler = new Mock<ILocalClipboardHandler>();
            _mockOmniClipboardHandler = new Mock<IOmniClipboardHandler>();

            _mockingKernel.Bind<ILocalClipboardHandler>().ToConstant(_mockLocalClipboardHandler.Object);
            _mockingKernel.Bind<IOmniClipboardHandler>().ToConstant(_mockOmniClipboardHandler.Object);
            _mockingKernel.Bind<IClipboadHandler>().To<ClipboardHandler>();

            _subject = _mockingKernel.Get<IClipboadHandler>();
        }

        [Test]
        public void Start_Always_SubscribesToLocalAndOmni()
        {
            _subject.Start();

            _mockLocalClipboardHandler.Verify(ch => ch.Subscribe(It.IsAny<IObserver<Clipping>>()));
            _mockOmniClipboardHandler.Verify(ch => ch.Subscribe(It.IsAny<IObserver<Clipping>>()));
        }

        [Test]
        public void Stop_Always_DisposesHandlers()
        {
            _mockLocalClipboardHandler.Setup(ch => ch.Subscribe(It.IsAny<IObserver<Clipping>>())).Returns(new Mock<IDisposable>().Object);
            _mockOmniClipboardHandler.Setup(ch => ch.Subscribe(It.IsAny<IObserver<Clipping>>())).Returns(new Mock<IDisposable>().Object);

            _subject.Start();
            _subject.Stop();

            _mockLocalClipboardHandler.Verify(ch => ch.Dispose());
            _mockOmniClipboardHandler.Verify(ch => ch.Dispose());
        }
    }
}