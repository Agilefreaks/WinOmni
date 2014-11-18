namespace ClipboardTests.Handlers
{
    using System.Reactive;
    using System.Reactive.Linq;
    using Clipboard.Handlers;
    using Clipboard.Models;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel;
    using NUnit.Framework;
    using OmniCommon.Helpers;
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

            _mockLocalClipboardHandler = new Mock<ILocalClipboardHandler> { DefaultValue = DefaultValue.Mock };
            _mockOmniClipboardHandler = new Mock<IOmniClipboardHandler> { DefaultValue = DefaultValue.Mock };

            _mockingKernel.Bind<ILocalClipboardHandler>().ToConstant(_mockLocalClipboardHandler.Object);
            _mockingKernel.Bind<IOmniClipboardHandler>().ToConstant(_mockOmniClipboardHandler.Object);
            _mockingKernel.Bind<IClipboardHandler>().To<ClipboardHandler>();

            _subject = _mockingKernel.Get<IClipboardHandler>();
        }

        [Test]
        public void Start_Always_StartsTheOmniClipboardHandlerWithTheGivenOmniMessageObservable()
        {
            var omniMessageObservable = Observable.Empty<OmniMessage>();
            _subject.Start(omniMessageObservable);

            _mockOmniClipboardHandler.Verify(x => x.Start(omniMessageObservable), Times.Once());
        }

        [Test]
        public void Start_Always_StartsTheLocalClipboardHandler()
        {
            _subject.Start(Observable.Empty<OmniMessage>());

            _mockLocalClipboardHandler.Verify(x => x.Start(), Times.Once());
        }

        [Test]
        public void Start_Always_ForwardsResultsFromLocalToOmni()
        {
            var testScheduler = new TestScheduler();
            SchedulerProvider.Default = testScheduler;
            var clipping = new Clipping("test");
            var localClippings = testScheduler.CreateColdObservable(
                new Recorded<Notification<Clipping>>(100, Notification.CreateOnNext(clipping)));
            _mockLocalClipboardHandler.Setup(x => x.Clippings).Returns(localClippings);

            _subject.Start(Observable.Empty<OmniMessage>());
            testScheduler.Start();

            _mockOmniClipboardHandler.Verify(x => x.PostClipping(clipping), Times.Once());
        }

        [Test]
        public void Start_Always_ForwardsResultsFromOmniToLocal()
        {
            var testScheduler = new TestScheduler();
            SchedulerProvider.Default = testScheduler;
            var clipping = new Clipping("test");
            var omniClippings = testScheduler.CreateColdObservable(
                new Recorded<Notification<Clipping>>(100, Notification.CreateOnNext(clipping)));
            _mockOmniClipboardHandler.Setup(x => x.Clippings).Returns(omniClippings);

            _subject.Start(Observable.Empty<OmniMessage>());
            testScheduler.Start();

            _mockLocalClipboardHandler.Verify(x => x.PostClipping(clipping), Times.Once());
        }

        [Test]
        public void Stop_Always_StopsHandlers()
        {
            _subject.Stop();

            _mockLocalClipboardHandler.Verify(ch => ch.Stop());
            _mockOmniClipboardHandler.Verify(ch => ch.Stop());
        }
    }
}