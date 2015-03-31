namespace ContactsTests.Handlers
{
    using System.Collections.Generic;
    using System.Reactive.Subjects;
    using Contacts.Api.Resources.v1;
    using Contacts.Handlers;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Models;

    [TestFixture]
    public class ContactCreatedHandlerTests
    {
        private MoqMockingKernel _mockingKernel;

        private IContactCreatedHandler _subject;

        private Mock<IContacts> _mockContactsResource;

        [SetUp]
        public void SetUp()
        {
            _mockContactsResource = new Mock<IContacts>();

            _mockingKernel = new MoqMockingKernel();
            _mockingKernel.Bind<IContacts>().ToConstant(_mockContactsResource.Object);
            _mockingKernel.Bind<IContactCreatedHandler>().To<ContactCreatedHandler>();
            _subject = _mockingKernel.Get<IContactCreatedHandler>();
        }

        [Test]
        public void WhenContactCreatedMessageArrives_GetsTheContactWithTheSpecifiedId()
        {
            var omniMessageObservable = new Subject<OmniMessage>();
            _subject.Start(omniMessageObservable);
            var payload = new Dictionary<string, string> { { "id", "1234"}};

            omniMessageObservable.OnNext(new OmniMessage { Type = "contact_created", Payload = payload});

            _mockContactsResource.Verify(cr => cr.Get("1234"), Times.Once);
        }

        [Test]
        public void WhenContactUpdatedMessageArrives_GetsTheContactWithTheSpecifiedId()
        {
            var omniMessageObservable = new Subject<OmniMessage>();
            _subject.Start(omniMessageObservable);
            var payload = new Dictionary<string, string> { { "id", "1234"}};

            omniMessageObservable.OnNext(new OmniMessage { Type = "contact_updated", Payload = payload});

            _mockContactsResource.Verify(cr => cr.Get("1234"), Times.Once);
        }
    }
}