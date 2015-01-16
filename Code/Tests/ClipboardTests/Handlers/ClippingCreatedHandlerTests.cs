namespace ClipboardTests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using Clipboard.API.Resources.v1;
    using Clipboard.Handlers;
    using Clipboard.Models;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    [TestFixture]
    public class ClippingCreatedHandlerTests
    {
        private IOmniClipboardHandler _omniClipboardHandler;

        private MoqMockingKernel _mockingKernel;

        private Mock<IClippings> _mockClippings;

        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void SetUp()
        {
            _mockingKernel = new MoqMockingKernel();
            _mockingKernel.Bind<IntPtr>().ToConstant(IntPtr.Zero);

            _mockClippings = _mockingKernel.GetMock<IClippings>();
            _mockConfigurationService = _mockingKernel.GetMock<IConfigurationService>();

            _mockingKernel.Bind<IOmniClipboardHandler>().To<ClippingCreatedHandler>();

            _omniClipboardHandler = _mockingKernel.Get<IOmniClipboardHandler>();
        }

        [Test]
        public void WhenAClippingMessageArrives_SubscriberOnNextIsCalled()
        {
            var observer = new Mock<IObserver<Clipping>>();
            var omniMessageObservable = new Subject<OmniMessage>();
            var clipping = new Clipping();
            _mockClippings.Setup(c => c.Get("42")).Returns(Observable.Return(clipping));
            _omniClipboardHandler.Start(omniMessageObservable);
            _omniClipboardHandler.Clippings.Subscribe(observer.Object);
            DispatcherProvider.Instance = new ImmediateDispatcherProvider();
            var autoResetEvent = new AutoResetEvent(false);
            observer.Setup(o => o.OnNext(clipping)).Callback(() => autoResetEvent.Set());

            omniMessageObservable.OnNext(new OmniMessage { Type = "clipping_created", Payload = new Dictionary<string, string> { { "id", "42" } } });

            autoResetEvent.WaitOne(1000);
            observer.Verify(o => o.OnNext(clipping), Times.Once);
        }

        [Test]
        public void WhenOtherMessageArrive_SubscriberOnNextIsNotCalled()
        {
            var observer = new Mock<IObserver<Clipping>>();
            var observable = new Subject<OmniMessage>();

            _omniClipboardHandler.Clippings.Subscribe(observer.Object);

            observable.OnNext(new OmniMessage { Type = "other" });

            observer.Verify(o => o.OnNext(It.IsAny<Clipping>()), Times.Never);
        }

        [Test]
        public void PostClipping_Always_PostsToApi()
        {
            _mockClippings.Setup(m => m.Create(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Observable.Empty<Clipping>());
            _mockConfigurationService.Setup(cs => cs.DeviceId).Returns("Radio");

            _omniClipboardHandler.PostClipping(new Clipping("some Content"));

            _mockClippings.Verify(ca => ca.Create("Radio", "some Content"));
        }
    }
}
