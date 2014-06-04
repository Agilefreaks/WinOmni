namespace ClipboardTests.Handlers
{
    using System;
    using System.Net;
    using System.Reactive.Subjects;
    using Clipboard.API;
    using Clipboard.Handlers;
    using Clipboard.Models;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using RestSharp;

    [TestFixture]
    public class OmniClipboardHandlerTests
    {
        private IOmniClipboardHandler _omniClipboardHandler;

        private IOmniMessageHandler _omniMessageHandler;

        private MoqMockingKernel _mockingKernel;

        private Mock<IClippingsApi> _mockClippingsApi;

        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void SetUp()
        {
            _mockingKernel = new MoqMockingKernel();
            _mockingKernel.Bind<IntPtr>().ToConstant(IntPtr.Zero);

            _mockClippingsApi = new Mock<IClippingsApi>();
            _mockConfigurationService = new Mock<IConfigurationService>();

            _mockingKernel.Bind<IConfigurationService>().ToConstant(_mockConfigurationService.Object);
            _mockingKernel.Bind<IClippingsApi>().ToConstant(_mockClippingsApi.Object);
            _mockingKernel.Bind<IOmniClipboardHandler, IOmniMessageHandler>()
                .To<OmniClipboardHandler>()
                .InSingletonScope();

            _omniClipboardHandler = _mockingKernel.Get<IOmniClipboardHandler>();
            _omniMessageHandler = _mockingKernel.Get<IOmniMessageHandler>();
        }

        [Test]
        public void WhenAClippingMessageArrives_SubscriberOnNextIsCalled()
        {
            var observer = new Mock<IObserver<Clipping>>();
            var observable = new Subject<OmniMessage>();
            var clipping = new Clipping();

            _mockClippingsApi
                .Setup(c => c.Last())
                .ReturnsAsync(new RestResponse<Clipping> { StatusCode = HttpStatusCode.OK, Data = clipping });

            _omniMessageHandler.SubscribeTo(observable);
            _omniClipboardHandler.Subscribe(observer.Object);

            observable.OnNext(new OmniMessage(OmniMessageTypeEnum.Clipboard));

            observer.Verify(o => o.OnNext(clipping), Times.Once);
        }

        [Test]
        public void WhenANotificationMessageArrives_SubscriberOnNextIsNotCalled()
        {
            var observer = new Mock<IObserver<Clipping>>();
            var observable = new Subject<OmniMessage>();

            _omniMessageHandler.SubscribeTo(observable);
            _omniClipboardHandler.Subscribe(observer.Object);

            observable.OnNext(new OmniMessage(OmniMessageTypeEnum.Notification));

            observer.Verify(o => o.OnNext(It.IsAny<Clipping>()), Times.Never);
        }

        [Test]
        public void PostClipping_Always_PostsToApi()
        {
            _mockConfigurationService.Setup(cs => cs.GetDeviceIdentifier()).Returns("Radio");

            _omniClipboardHandler.PostClipping(new Clipping("some content"));

            _mockClippingsApi.Verify(ca => ca.PostClipping("Radio", "some content"));
        }
    }
}
