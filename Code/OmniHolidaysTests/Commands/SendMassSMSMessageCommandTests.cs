namespace OmniHolidaysTests.Commands
{
    using System.Collections.Generic;
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniHolidays.Commands;
    using OmniHolidays.Services;
    using OmniUI.Models;
    using SMS.Resources.v1;

    [TestFixture]
    public class SendMassSMSMessageCommandTests
    {
        private SendMassSMSMessageCommand _subject;

        private Mock<ISMSMessages> _mockSMSMessages;

        private TestScheduler _testScheduler;

        private Mock<ITemplateProcessingService> _mockTemplateProcessingService;

        [SetUp]
        public void Setup()
        {
            _mockSMSMessages = new Mock<ISMSMessages>();
            _mockTemplateProcessingService = new Mock<ITemplateProcessingService> { DefaultValue = DefaultValue.Mock };
            _testScheduler = new TestScheduler();

        }

        [Test]
        public void Execute_Always_SendsSMSMessagesToAllTheGivenContactsUsingTheGivenTemplate()
        {
            const string Template = "%Hello %ContactFirstName%, my name is %UserLastName%";
            var contacts = new List<IContactInfo>
                               {
                                   new ContactInfo { FirstName = "testA", Phone = "1234" },
                                   new ContactInfo { FirstName = "testB", Phone = "7890" }
                               };
            var counter = 0;
            _mockTemplateProcessingService.Setup(
                x => x.Process(Template, It.Is<ContactInfo>(contactInfo => contacts.Contains(contactInfo))))
                .Returns(() => "someResult" + counter++);
            _subject = new SendMassSMSMessageCommand(Template, contacts)
            {
                TemplateProcessingService = _mockTemplateProcessingService.Object,
                SMSMessages = _mockSMSMessages.Object
            };
            var sendObservable = Observable.Return(new EmptyModel());
            _mockSMSMessages.Setup(x => x.Send(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(sendObservable);

            _testScheduler.Start(_subject.Execute);

            var expectedMessages = new[] { "someResult0", "someResult1" };
            var expectedPhoneNumbers = new[] { "1234", "7890" };
            _mockSMSMessages.Verify(x => x.Send(expectedMessages, expectedPhoneNumbers));
        }

        [Test]
        public void Execute_SendIsSuccessful_EmitsAUnit()
        {
            var template = string.Empty;
            var contacts = new List<IContactInfo>();
            _subject = new SendMassSMSMessageCommand(template, contacts)
            {
                TemplateProcessingService = _mockTemplateProcessingService.Object,
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