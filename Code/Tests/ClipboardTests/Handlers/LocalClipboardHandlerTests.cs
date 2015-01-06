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
        public void Start_Always_SubscribesToTheClipboardWrapper()
        {
            _subject.Start();

            _mockWindowsClipboardWrapper.Verify(wc => wc.Subscribe(_subject), Times.Once);
        }

        [Test]
        public void Subscribe_AfterStart_CreatesClippingsFromTheClipboardEventsAndPassesThemToSubscribers()
        {
            _mockWindowsClipboardWrapper
                .Setup(wcw => wcw.Subscribe((_subject)))
                .Callback<IObserver<ClipboardEventArgs>>(o => _clipboardEventsStream.Subscribe(o));
            Clipping clipping = null;
            _subject.Start();
            _subject.Clippings.Subscribe(c => clipping = c);

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
            _subject.Clippings.Subscribe(observer.Object);

            _mockWindowsClipboardWrapper.Setup(m => m.SetData(It.IsAny<string>()))
                .Callback(() => _mockWindowsClipboardWrapper.Raise(m => m.DataReceived += null, new ClipboardEventArgs() { Data = "42" }));

            _subject.PostClipping(new Clipping() { Content = "42" });

            observer.Verify(m => m.OnNext(It.IsAny<Clipping>()), Times.Never);
        }

        [Test]
        public void OnNext_AfterAPostClippingWithTheSameContentFollowedByAOnNextWithDifferentContent_ForwardsTheEvent()
        {
            const string Content = "some content";
            var testableObserver = _testScheduler.CreateObserver<Clipping>();
            _subject.Clippings.Subscribe(testableObserver);
            _subject.PostClipping(new Clipping(Content));
            _subject.OnNext(new ClipboardEventArgs("some other content"));

            _subject.OnNext(new ClipboardEventArgs(Content));

            testableObserver.Messages.Count.Should().Be(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.Content.Should().Be("some other content");
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[1].Value.Value.Content.Should().Be("some content");
        }

        [Test]
        public void OnNext_AfterAPostClippingWithTheSameContent_DoesNotForwardTheEvent()
        {
            const string Content = "some content";
            var testableObserver = _testScheduler.CreateObserver<Clipping>();
            _subject.Clippings.Subscribe(testableObserver);
            _subject.PostClipping(new Clipping(Content));

            _subject.OnNext(new ClipboardEventArgs(Content));

            testableObserver.Messages.Count.Should().Be(0);
        }
    }
}