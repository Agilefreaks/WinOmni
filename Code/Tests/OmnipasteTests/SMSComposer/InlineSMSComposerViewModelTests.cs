namespace OmnipasteTests.SMSComposer
{
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Services;
    using Omnipaste.SMSComposer;
    using OmniUI.Models;

    [TestFixture]
    public class InlineSMSComposerViewModelTests
    {
        private IInlineSMSComposerViewModel _subject;

        private Mock<ISMSMessageFactory> _mockSMSMessageFactory;

        private Mock<IDevices> _mockDevices;

        private TestScheduler _testScheduler;

        private Mock<IMessageStore> _mockMessageStore;

        [SetUp]
        public void Setup()
        {
            _mockSMSMessageFactory = new Mock<ISMSMessageFactory> { DefaultValue = DefaultValue.Mock };
            _mockDevices = new Mock<IDevices>();
            _mockMessageStore = new Mock<IMessageStore>();
            _subject = new InlineSMSComposerViewModel(_mockDevices.Object, _mockSMSMessageFactory.Object)
                           {
                               MessageStore = _mockMessageStore.Object
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
            _mockDevices.Setup(x => x.SendSms("1234", "test")).Returns(sendObservable);
            var contactInfo = new ContactInfo();
            _subject.ContactInfo = contactInfo;

            _subject.Send();
            _testScheduler.Start();

            _mockDevices.Verify(x => x.SendSms("1234", "test"), Times.Once());
            _mockSMSMessageFactory.Verify(x => x.Create(contactInfo), Times.Once());
        }

        [Test]
        public void SendSMS_Always_AddsTheSentSMSToTheMessageStore()
        {
            var sendObservable = Observable.Return(new EmptyModel());
            var message = new Message();
            _subject.Model = new SMSMessage(message);
            _mockDevices.Setup(x => x.SendSms(It.IsAny<string>(), It.IsAny<string>())).Returns(sendObservable);

            _subject.Send();
            _testScheduler.Start();

            _mockMessageStore.Verify(x => x.AddMessage(message));
        }
    }
}