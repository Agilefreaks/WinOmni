namespace OmniHolidaysTests.Commands
{
    using System.Collections.Generic;
    using System.Reactive;
    using System.Reactive.Linq;
    using Contacts.Models;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using OmniHolidays.Commands;
    using SMS.Resources.v1;

    [TestFixture]
    public class SendMassSMSMessageCommandTests
    {
        private SendMassSMSMessageCommand _subject;

        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<ISMSMessages> _mockSMSMessages;

        private TestScheduler _testScheduler;

        [SetUp]
        public void Setup()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockSMSMessages = new Mock<ISMSMessages>();
            _testScheduler = new TestScheduler();
        }

        [Test]
        public void Execute_Always_SendsSMSMessagesToAllTheGivenContactsUsingTheGivenTemplate()
        {
            const string Template = "%Hello %ContactFirstName%, my name is %UserLastName%";
            var contacts = new List<Contact>
                               {
                                   new Contact { FirstName = "testA", PhoneNumber = "1234" },
                                   new Contact { FirstName = "testB", PhoneNumber = "7890" }
                               };
            _mockConfigurationService.SetupGet(x => x.UserInfo).Returns(new UserInfo { LastName = "testC" });
            _subject = new SendMassSMSMessageCommand(Template, contacts)
            {
                ConfigurationService = _mockConfigurationService.Object,
                SMSMessages = _mockSMSMessages.Object
            };
            var sendObservable = Observable.Return(new EmptyModel());
            _mockSMSMessages.Setup(x => x.Send(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(sendObservable);

            _testScheduler.Start(_subject.Execute);

            var expectedMessages = new[] { "%Hello testA, my name is testC", "%Hello testB, my name is testC" };
            var expectedPhoneNumbers = new[] { "1234", "7890" };
            _mockSMSMessages.Verify(x => x.Send(expectedMessages, expectedPhoneNumbers));
        }

        [Test]
        public void Execute_SendIsSuccessful_EmitsAUnit()
        {
            var template = string.Empty;
            var contacts = new List<Contact>();
            _subject = new SendMassSMSMessageCommand(template, contacts)
            {
                ConfigurationService = _mockConfigurationService.Object,
                SMSMessages = _mockSMSMessages.Object
            };
            var sendObservable = Observable.Return(new EmptyModel());
            _mockSMSMessages.Setup(x => x.Send(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(sendObservable);

            var testableObserver = _testScheduler.Start(_subject.Execute);

            testableObserver.Messages.Count.Should().Be(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.Should().NotBeNull();
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }
    }
}