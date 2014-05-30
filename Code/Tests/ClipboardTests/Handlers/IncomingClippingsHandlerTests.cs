namespace ClipboardTests.Handlers
{
    using System;
    using System.Net;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using WindowsClipboard;
    using Caliburn.Micro;
    using Clipboard.API;
    using Clipboard.Handlers;
    using Clipboard.Models;
    using FluentAssertions;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Models;
    using RestSharp;

    [TestFixture]
    public class IncomingClippingsHandlerTests
    {
        private IncomingClippingsHandler _subject;

        private MoqMockingKernel _mockingKernel;

        private IEventAggregator _eventAggregator;

        private Mock<IClippingsApi> _mockClippingsApi;

        private class TestSubscriber : IHandle<ClipboardData>
        {
            public bool Called { get; private set; }

            public TestSubscriber()
            {
                Called = false;
            }

            public void Handle(ClipboardData message)
            {
                Called = true;
            }
        }

        [SetUp]
        public void SetUp()
        {
            _mockingKernel = new MoqMockingKernel();
            _mockingKernel.Bind<IntPtr>().ToConstant(IntPtr.Zero);

            _eventAggregator = new EventAggregator();
            _mockingKernel.Bind<IEventAggregator>().ToConstant(_eventAggregator);

            _mockClippingsApi = _mockingKernel.GetMock<IClippingsApi>();

            _subject = _mockingKernel.Get<IncomingClippingsHandler>();
        }

        [Test]
        public void HasClippingsApi()
        {
            Assert.IsNotNull(_subject.ClippingsApi);
        }

        [Test]
        public void IsSubscribedToTheEventAggregator()
        {
            Assert.AreSame(_subject.EventAggregator, _eventAggregator);
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
            _mockClippingsApi
                .Setup(c => c.Last())
                .Returns(() => taskResponse);

            _subject.OnNext(new OmniMessage());

            _mockClippingsApi.Verify(c => c.Last(), Times.Once());
        }

        [Test]
        public void OnNext_WhenLastClippingIsSuccessful_PublishesItToTheEventAggregator()
        {
            var clipping = new Clipping("content");
            IRestResponse<Clipping> lastClippingResponse = new RestResponse<Clipping> { StatusCode = HttpStatusCode.OK, Data = clipping };
            _mockClippingsApi.Setup(c => c.Last()).Returns(Task.Factory.StartNew(() => lastClippingResponse));
            TestSubscriber testSubscriber = new TestSubscriber();
            _eventAggregator.Subscribe(testSubscriber);

            _subject.OnNext(new OmniMessage());

            testSubscriber.Called.Should().BeTrue("the handler was not called");
        }

        [Test]
        public void OnNext_WhenLastClippingIsNotSuccessful_DoesNotPublishTheClippingOnTheEventAggregator()
        {
            IRestResponse<Clipping> lastClippingResponse = new RestResponse<Clipping> { StatusCode = HttpStatusCode.Forbidden, Data = new Clipping() };
            _mockClippingsApi.Setup(c => c.Last()).Returns(Task.Factory.StartNew(() => lastClippingResponse));
            TestSubscriber testSubscriber = new TestSubscriber();

            _eventAggregator.Subscribe(testSubscriber);
            _subject.OnNext(new OmniMessage());

            testSubscriber.Called.Should().BeFalse();
        }

        [Test]
        public void SubscribeTo_Always_IsSubscribedToClippingsMessages()
        {
            var subject = new Subject<OmniMessage>();
            IRestResponse<Clipping> lastClippingResponse = new RestResponse<Clipping>();
            _mockClippingsApi.Setup(c => c.Last()).Returns(Task.Factory.StartNew(() => lastClippingResponse));

            _subject.SubscribeTo(subject);
            subject.OnNext(new OmniMessage { Provider = OmniMessageTypeEnum.Clipboard });

            _mockClippingsApi.Verify(c => c.Last(), Times.Once());
        }

        [Test]
        public void SubscribeTo_Never_HandlesOtherTypesOfMessages()
        {
            var subject = new Subject<OmniMessage>();

            _subject.SubscribeTo(subject);
            subject.OnNext(new OmniMessage { Provider = OmniMessageTypeEnum.Phone });

            _mockClippingsApi.Verify(c => c.Last(), Times.Never());
        }
    }
}
