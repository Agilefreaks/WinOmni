﻿namespace ClipboardTests.Handlers
{
    using System;
    using System.Reactive.Subjects;
    using Clipboard.API;
    using Clipboard.Handlers;
    using Clipboard.Handlers.WindowsClipboard;
    using Clipboard.Models;
    using FluentAssertions;
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
            _subject.Subscribe(new Mock<IObserver<Clipping>>().Object);

            _mockWindowsClipboardWrapper.Verify(wc => wc.StartWatchingClipboard(), Times.Once);
        }

        [Test]
        public void Subscribe_OnlySubscribesOnceToTheClipboardDataReceivedEvent()
        {
            int clippingsReceived = 0;
            
            _subject.Subscribe(c => clippingsReceived++);
            _subject.Subscribe(c => clippingsReceived++);

            _mockWindowsClipboardWrapper.Raise(x => x.DataReceived += null, new ClipboardEventArgs("clipping content"));

            clippingsReceived.Should().Be(2);
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