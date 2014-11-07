namespace OmnipasteTests.SmsComposer
{
    using System;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Properties;
    using Omnipaste.SmsComposer;

    [TestFixture]
    public class SMSFactoryTests
    {
        private SMSFactory _subject;

        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void Setup()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            _subject = new SMSFactory(_mockConfigurationService.Object);
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

        [Test]
        public void Create_IsSMSSuffixEnabled_ReturnsAMessageWithJustTheOmnipasteBranding()
        {
            _mockConfigurationService.SetupGet(x => x.IsSMSSuffixEnabled).Returns(true);

            _subject.Create().Message.Should().Be(Environment.NewLine + Resources.SentFromOmnipaste);
        }

        [Test]
        public void CreateWithMessage_IsSMSSuffixEnabled_ReturnsAMessageSuffixedWithTheOmnipasteBranding()
        {
            _mockConfigurationService.SetupGet(x => x.IsSMSSuffixEnabled).Returns(true);
            var sendSmsMessage = new SendSmsMessage { Message = "testA"};

            _subject.Create(sendSmsMessage)
                .Message.Should()
                .Be("testA" + Environment.NewLine + Resources.SentFromOmnipaste);
        }
    }
}