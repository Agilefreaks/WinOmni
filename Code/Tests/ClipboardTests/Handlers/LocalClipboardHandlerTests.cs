namespace ClipboardTests.Handlers
{
    using System;
    using System.Reactive;
    using Clipboard.API.Resources.v1;
    using Clipboard.Handlers;
    using Clipboard.Handlers.WindowsClipboard;
    using Clipboard.Models;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
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

        private Mock<IClippings> _mockClippings;

        private TestScheduler _testScheduler;

        private ITestableObservable<ClipboardEventArgs> _clipboardEventsStream;

        [SetUp]
        public void SetUp()
        {
            _mockingKernel = new MockingKernel();
            _mockWindowsClipboardWrapper = new Mock<IWindowsClipboardWrapper> { DefaultValue = DefaultValue.Mock };
            _mockClippings = new Mock<IClippings>();

            _mockingKernel.Bind<IWindowsClipboardWrapper>().ToConstant(_mockWindowsClipboardWrapper.Object);
            _mockingKernel.Bind<IClippings>().ToConstant(_mockClippings.Object);

            _mockingKernel.Bind<ILocalClipboardHandler>().To<LocalClipboardHandler>();

            _testScheduler = new TestScheduler();
            _clipboardEventsStream = _testScheduler.CreateColdObservable(
                new Recorded<Notification<ClipboardEventArgs>>(1, Notification.CreateOnNext(new ClipboardEventArgs { Data = "clipping content"})));

            _subject = _mockingKernel.Get<ILocalClipboardHandler>();
        }

        [Test]
        public void Subscribe_Always_StartsSubscribesToTheClipboardWrapper()
        {
            _subject.Subscribe(new Mock<IObserver<Clipping>>().Object);

            _mockWindowsClipboardWrapper.Verify(wc => wc.Subscribe(_subject), Times.Once);
        }

        [Test]
        public void Subscribe_CreatesClippingsFromTheClipboardEventsAndPassesThemToSubscribers()
        {
            _mockWindowsClipboardWrapper
                .Setup(wcw => wcw.Subscribe((_subject)))
                .Callback<IObserver<ClipboardEventArgs>>(o => _clipboardEventsStream.Subscribe(o));
            Clipping clipping = null;
            _subject.Subscribe(c => clipping = c);

            _testScheduler.Start();

            clipping.Content.Should().Be("clipping content");
        }

        [Test]
        public void PostClipping_Always_CallsPostClipping()
        {
            _subject.PostClipping(new Clipping("some stuff"));

            _mockWindowsClipboardWrapper.Verify(wc => wc.SetData("some stuff"), Times.Once);
        }

        [Test]
        public void PostClipping_DoesNotCallOnNext()
        {
            var observer = new Mock<IObserver<Clipping>>();
            _subject.Subscribe(observer.Object);

            _mockWindowsClipboardWrapper.Setup(m => m.SetData(It.IsAny<string>()))
                .Callback(() => _mockWindowsClipboardWrapper.Raise(m => m.DataReceived += null, new ClipboardEventArgs() { Data = "42" }));

            _subject.PostClipping(new Clipping() { Content = "42" });

            observer.Verify(m => m.OnNext(It.IsAny<Clipping>()), Times.Never);
        }
    }
}