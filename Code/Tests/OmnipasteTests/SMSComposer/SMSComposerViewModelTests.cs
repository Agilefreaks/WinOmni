namespace OmnipasteTests.SMSComposer
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using Omnipaste.SMSComposer;
    using Omnipaste.Properties;
    using SMS.Models;
    using SMS.Resources.v1;

    [TestFixture]
    public class SMSComposerViewModelTests
    {
        private ISMSComposerViewModel _subject;

        private Mock<ISMSMessages> _mockSMSMessages;

        private TestScheduler _testScheduler;

        private Mock<IMessageRepository> _mockMessageRepository;

        private Mock<IConfigurationService> _mockConfigurationService;

        private ContactInfo _contactInfo;

        [SetUp]
        public void Setup()
        {
            _mockSMSMessages = new Mock<ISMSMessages>();
            _mockMessageRepository = new Mock<IMessageRepository>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _contactInfo = new ContactInfo { Phone = "1234", FirstName = "F", LastName = "L" };
            _subject = new SMSComposerViewModel(_mockSMSMessages.Object, _mockConfigurationService.Object)
                           {
                               ContactInfo = _contactInfo,
                               MessageRepository = _mockMessageRepository.Object
                           };
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void Activate_WhenIsSMSSuffixEnabledIsFalse_PopulatesMessageWithEmptyString()
        {
            _mockConfigurationService.SetupGet(x => x.IsSMSSuffixEnabled).Returns(false);

            _subject.Activate();

            _subject.Message.Should().Be(string.Empty);
        }

        [Test]
        public void Activate_WhenIsSMSSuffixEnabledIsTrue_PopulatesMessageWithTheOmnipasteBranding()
        {
            _mockConfigurationService.SetupGet(x => x.IsSMSSuffixEnabled).Returns(true);

            _subject.Activate();

            _subject.Message.Should().Be(Environment.NewLine + Resources.SentFromOmnipaste);
        }

        [Test]
        public void SendSMS_Always_SetsTheModelToANewSMSMessageAfterSending()
        {
            var sendObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<SmsMessage>>(100, Notification.CreateOnNext(new SmsMessage())),
                new Recorded<Notification<SmsMessage>>(200, Notification.CreateOnCompleted<SmsMessage>()));
            _subject.Message = "test";
            _mockSMSMessages.Setup(x => x.Send("1234", "test")).Returns(sendObservable);

            _subject.Send();
            _testScheduler.Start();

            _mockSMSMessages.Verify(x => x.Send("1234", "test"), Times.Once());
        }

        [Test]
        public void SendSMS_Always_AddsTheSentSMSToTheMessageStore()
        {
            const string Content = "Test";
            var sendObservable = Observable.Return(new SmsMessage { Content = Content }, _testScheduler);
            _mockSMSMessages.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<string>())).Returns(sendObservable);
            _subject.Message = Content;

            _subject.Send();
            _testScheduler.Start();

            _mockMessageRepository.Verify(x => x.Save(It.Is<Message>(m => m.Content == Content)));
        }
    }
}