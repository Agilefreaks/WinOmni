namespace OmnipasteTests.SmsComposer
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.SmsComposer;

    [TestFixture]
    public class SMSFactoryTests
    {
        private SMSFactory _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new SMSFactory();
        }

        [Test]
        public void Create_Always_ReturnsMessageWithEmptyContent()
        {
            _subject.Create().Message.Should().Be(string.Empty);
        }

        [Test]
        public void CreateWithSendSmsMessage_Always_ReturnsMessageWithGivenContent()
        {
            var sendSmsMessage = new SendSmsMessage { Message = "testA" };

            _subject.Create(sendSmsMessage).Message.Should().Be("testA");
        }

        [Test]
        public void CreateWithSendSmsMessage_Always_ReturnsMessageWithGivenRecipient()
        {
            var sendSmsMessage = new SendSmsMessage { Recipient = "testB" };

            _subject.Create(sendSmsMessage).Recipient.Should().Be("testB");
        }
    }
}