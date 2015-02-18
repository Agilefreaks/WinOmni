namespace OmnipasteTests.SMSComposer
{
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using Omnipaste.SMSComposer;
    using SMS.Resources.v1;

    [TestFixture]
    public class SMSComposerViewModelTests
    {
        private ISMSComposerViewModel _subject;

        private Mock<ISMSMessageFactory> _mockSMSMessageFactory;

        private Mock<ISMSMessages> _mockSMSMessages;

        private TestScheduler _testScheduler;

        private Mock<IMessageRepository> _mockMessageRepository;

        [SetUp]
        public void Setup()
        {
            _mockSMSMessageFactory = new Mock<ISMSMessageFactory> { DefaultValue = DefaultValue.Mock };
            _mockSMSMessages = new Mock<ISMSMessages>();
            _mockMessageRepository = new Mock<IMessageRepository>();
            _subject = new SMSComposerViewModel(_mockSMSMessages.Object, _mockSMSMessageFactory.Object)
                           {
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
        public void Activate_Always_CreatesASMSMessageUsingTheContactInfoSetOnTheViewModelAndSetsItAsItsModel()
        {
            var contactInfo = new ContactInfo();
            _subject.ContactInfo = contactInfo;
            var smsMessage = new SMSMessage();
            _mockSMSMessageFactory.Setup(x => x.Create(contactInfo)).Returns(smsMessage);

            _subject.Activate();

            _mockSMSMessageFactory.Verify(x => x.Create(contactInfo), Times.Once());
            _subject.Model.Should().Be(smsMessage);
        }

        [Test]
        public void SendSMS_Always_SetsTheModelToANewSMSMessageAfterSending()
        {
            _testScheduler.CreateColdObservable(
                new Recorded<Notification<EmptyModel>>(100, Notification.CreateOnNext(new EmptyModel())),
                new Recorded<Notification<EmptyModel>>(200, Notification.CreateOnCompleted<EmptyModel>()));
            var sendObservable = Observable.Return(new EmptyModel());
            _subject.Model = new SMSMessage { Message = "test", Recipient = "1234" };
            _mockSMSMessages.Setup(x => x.Send("1234", "test")).Returns(sendObservable);
            var contactInfo = new ContactInfo();
            _subject.ContactInfo = contactInfo;

            _subject.Send();
            _testScheduler.Start();

            _mockSMSMessages.Verify(x => x.Send("1234", "test"), Times.Once());
            _mockSMSMessageFactory.Verify(x => x.Create(contactInfo), Times.Once());
        }

        [Test]
        public void SendSMS_Always_AddsTheSentSMSToTheMessageStore()
        {
            var sendObservable = Observable.Return(new EmptyModel());
            var message = new Message();
            _subject.Model = new SMSMessage(message);
            _mockSMSMessages.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<string>())).Returns(sendObservable);

            _subject.Send();
            _testScheduler.Start();

            _mockMessageRepository.Verify(x => x.Save(message));
        }
    }
}