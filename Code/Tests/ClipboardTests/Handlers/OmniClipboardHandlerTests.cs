namespace ClipboardTests.Handlers
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Clipboard.API.Resources.v1;
    using Clipboard.Handlers;
    using Clipboard.Models;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    [TestFixture]
    public class OmniClipboardHandlerTests
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

            _mockingKernel.Bind<IOmniClipboardHandler>().To<OmniClipboardHandler>();

            _omniClipboardHandler = _mockingKernel.Get<IOmniClipboardHandler>();
        }

        [Test]
        public void WhenAClippingMessageArrives_SubscriberOnNextIsCalled()
        {
            var observer = new Mock<IObserver<Clipping>>();
            var observable = new Subject<OmniMessage>();
            var clipping = new Clipping();

            _mockClippings
                .Setup(c => c.Last())
                .Returns(Observable.Return(clipping));

            _omniClipboardHandler.SubscribeTo(observable);
            _omniClipboardHandler.Subscribe(observer.Object);

            observable.OnNext(new OmniMessage(OmniMessageTypeEnum.Clipboard));

            observer.Verify(o => o.OnNext(clipping), Times.Once);
        }

        [Test]
        public void WhenANotificationMessageArrives_SubscriberOnNextIsNotCalled()
        {
            var observer = new Mock<IObserver<Clipping>>();
            var observable = new Subject<OmniMessage>();

            _omniClipboardHandler.Subscribe(observer.Object);

            observable.OnNext(new OmniMessage(OmniMessageTypeEnum.Notification));

            observer.Verify(o => o.OnNext(It.IsAny<Clipping>()), Times.Never);
        }

        [Test]
        public void PostClipping_Always_PostsToApi()
        {
            _mockClippings.Setup(m => m.Create(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Observable.Empty<Clipping>());
            _mockConfigurationService.Setup(cs => cs.DeviceIdentifier).Returns("Radio");

            _omniClipboardHandler.PostClipping(new Clipping("some Content"));

            _mockClippings.Verify(ca => ca.Create("Radio", "some Content"));
        }
    }
}
