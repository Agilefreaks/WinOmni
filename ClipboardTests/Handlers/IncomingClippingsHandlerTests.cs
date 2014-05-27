using System;
using System.Net;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using WindowsClipboard;
using Caliburn.Micro;
using Clipboard;
using Clipboard.Handlers;
using Clipboard.Models;
using Moq;
using Ninject;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;
using OmniCommon.Models;
using RestSharp;

namespace ClipboardTests.Handlers
{
    [TestFixture]
    public class IncomingClippingsHandlerTests
    {
        private IncomingClippingsHandler _subject;

        private MoqMockingKernel _mockingKernel;

        private Mock<IEventAggregator> _mockEventAggregator;

        private Mock<IClippingsAPI> _mockClippingsAPI;

        [SetUp]
        public void SetUp()
        {
            _mockingKernel = new MoqMockingKernel();
            _mockingKernel.Bind<IntPtr>().ToConstant(IntPtr.Zero);
            
            _mockEventAggregator = _mockingKernel.GetMock<IEventAggregator>();
            _mockClippingsAPI = _mockingKernel.GetMock<IClippingsAPI>();

            _subject = _mockingKernel.Get<IncomingClippingsHandler>();
        }

        [Test]
        public void HasClippingsAPI()
        {
            Assert.IsNotNull(_subject.ClippingsAPI);
        }

        [Test]
        public void IsSubscribedToTheEventAggregator()
        {
            Assert.AreSame(_subject.EventAggregator, _mockEventAggregator.Object);
        }

        [Test]
        public void OnNext_Always_GetsTheLastClipping()
        {
            IRestResponse<Clipping> lastClippingResponse = new RestResponse<Clipping>
                                       {
                                           StatusCode = HttpStatusCode.OK,
                                           Data = new Clipping(string.Empty)
                                       };
            var taskResponse = Task.Factory.StartNew(() => lastClippingResponse);
            _mockClippingsAPI
                .Setup(c => c.Last())
                .Returns(() => taskResponse);

            _subject.OnNext(new OmniMessage());

            _mockClippingsAPI.Verify(c => c.Last(), Times.Once());
        }

        [Test]
        public void OnNext_WhenLastClippingIsSuccessful_PublishesItToTheEventAggregator()
        {
            var clipping = new Clipping("content");
            IRestResponse<Clipping> lastClippingResponse = new RestResponse<Clipping> { StatusCode = HttpStatusCode.OK, Data = clipping };
            _mockClippingsAPI.Setup(c => c.Last()).Returns(Task.Factory.StartNew(() => lastClippingResponse));

            _subject.OnNext(new OmniMessage());

            _mockEventAggregator.Verify(ea => ea.Publish(It.Is<ClipboardData>(c => c.GetData() == "content")));
        }

        [Test]
        public void OnNext_WhenLastClippingIsNotSuccessful_DoesNotPublishTheClippingOnTheEventAggregator()
        {
            IRestResponse<Clipping> lastClippingResponse = new RestResponse<Clipping> { StatusCode = HttpStatusCode.Forbidden, Data = new Clipping()};
            _mockClippingsAPI.Setup(c => c.Last()).Returns(Task.Factory.StartNew(() => lastClippingResponse));

            _subject.OnNext(new OmniMessage());

            _mockEventAggregator.Verify(ea => ea.Publish(It.IsAny<Clipping>()), Times.Never());
        }

        [Test]
        public void SubscribeTo_Always_IsSubscribedToClippingsMessages()
        {
            var subject = new Subject<OmniMessage>();
            IRestResponse<Clipping> lastClippingResponse = new RestResponse<Clipping>();
            _mockClippingsAPI.Setup(c => c.Last()).Returns(Task.Factory.StartNew(() => lastClippingResponse));

            _subject.SubscribeTo(subject);
            subject.OnNext(new OmniMessage {Provider = OmniMessageTypeEnum.Clipboard });

            _mockClippingsAPI.Verify(c => c.Last(), Times.Once());
        }

        [Test]
        public void SubscribeTo_Never_HandlesOtherTypesOfMessages()
        {
            var subject = new Subject<OmniMessage>();

            _subject.SubscribeTo(subject);
            subject.OnNext(new OmniMessage { Provider = OmniMessageTypeEnum.Phone });

            _mockClippingsAPI.Verify(c => c.Last(), Times.Never());
        }
    }
}
