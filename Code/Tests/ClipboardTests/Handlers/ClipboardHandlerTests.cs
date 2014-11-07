namespace ClipboardTests.Handlers
{
    using System;
    using System.Reactive.Linq;
    using Clipboard.Handlers;
    using Clipboard.Models;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel;
    using NUnit.Framework;
    using OmniCommon.Models;

    [TestFixture]
    public class ClipboardHandlerTests
    {
        private MockingKernel _mockingKernel;

        private Mock<ILocalClipboardHandler> _mockLocalClipboardHandler;

        private Mock<IOmniClipboardHandler> _mockOmniClipboardHandler;

        private IClipboardHandler _subject;

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MockingKernel();

            _mockLocalClipboardHandler = new Mock<ILocalClipboardHandler>();
            _mockOmniClipboardHandler = new Mock<IOmniClipboardHandler>();

            _mockingKernel.Bind<ILocalClipboardHandler>().ToConstant(_mockLocalClipboardHandler.Object);
            _mockingKernel.Bind<IOmniClipboardHandler>().ToConstant(_mockOmniClipboardHandler.Object);
            _mockingKernel.Bind<IClipboardHandler>().To<ClipboardHandler>();

            _subject = _mockingKernel.Get<IClipboardHandler>();
        }

        [Test]
        public void Start_Always_SubscribesToLocalAndOmni()
        {
            _subject.Start(Observable.Empty<OmniMessage>());

            _mockLocalClipboardHandler.Verify(ch => ch.Subscribe(It.IsAny<IObserver<Clipping>>()));
            _mockOmniClipboardHandler.Verify(ch => ch.Subscribe(It.IsAny<IObserver<Clipping>>()));
        }

        [Test]
        public void Stop_Always_DisposesHandlers()
        {
            _mockLocalClipboardHandler.Setup(ch => ch.Subscribe(It.IsAny<IObserver<Clipping>>())).Returns(new Mock<IDisposable>().Object);
            _mockOmniClipboardHandler.Setup(ch => ch.Subscribe(It.IsAny<IObserver<Clipping>>())).Returns(new Mock<IDisposable>().Object);

            _subject.Start(Observable.Empty<OmniMessage>());
            _subject.Stop();

            _mockLocalClipboardHandler.Verify(ch => ch.Dispose());
            _mockOmniClipboardHandler.Verify(ch => ch.Dispose());
        }

        [Test]
        public void Start_Always_SubscribesOmniClipboardToOmniMessageObservable()
        {
            var omniMessageObservable = Observable.Empty<OmniMessage>();

            _subject.Start(omniMessageObservable);

            _mockOmniClipboardHandler.Verify(m => m.Start(omniMessageObservable), Times.Once);
        }
    }
}