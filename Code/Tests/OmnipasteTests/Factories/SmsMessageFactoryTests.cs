namespace OmnipasteTests.Factories
{
    using System.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Factories;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using SMS.Models;

    [TestFixture]
    public class SmsMessageFactoryTests
    {
        private MoqMockingKernel _kernel;

        private ISmsMessageFactory _subject;

        private TestScheduler _scheduler;

        private Mock<ISmsMessageRepository> _mockMessageRepository;

        private Mock<IContactRepository> _mockContactRepository;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _mockMessageRepository = new Mock<ISmsMessageRepository> { DefaultValue = DefaultValue.Mock };
            _mockContactRepository = new Mock<IContactRepository> { DefaultValue = DefaultValue.Mock };

            _kernel.Bind<ISmsMessageRepository>().ToConstant(_mockMessageRepository.Object);
            _kernel.Bind<IContactRepository>().ToConstant(_mockContactRepository.Object);

            _kernel.Bind<ISmsMessageFactory>().To<SmsMessageFactory>();
            _subject = _kernel.Get<ISmsMessageFactory>();

            _scheduler = new TestScheduler();
            SchedulerProvider.Default = _scheduler;
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void Create_Always_GetOrCreateByPhoneNumberContact()
        {
            var smsMessageDto = new SmsMessageDto { PhoneNumber = "42" };

            _scheduler.Start(() => _subject.Create<RemoteSmsMessage>(smsMessageDto));

            _mockContactRepository.Verify(cr => cr.GetOrCreateByPhoneNumber(smsMessageDto.PhoneNumber), Times.Once);
        }

        [Test]
        public void Create_Always_UpdatesLastActivityTimeOnTheContact()
        {
            var smsMessageDto = new SmsMessageDto();
            var contactInfo = new ContactInfo();
            var contactInfoObservable = _scheduler.CreateColdObservable(new Recorded<System.Reactive.Notification<ContactInfo>>(100, System.Reactive.Notification.CreateOnNext(contactInfo)));
            _mockContactRepository.Setup(cr => cr.GetOrCreateByPhoneNumber(It.IsAny<string>())).Returns(contactInfoObservable);

            _scheduler.Start(() => _subject.Create<RemoteSmsMessage>(smsMessageDto));

            _mockContactRepository.Verify(cr => cr.UpdateLastActivityTime(contactInfo, null), Times.Once);            
        }

        [Test]
        public void Create_Always_SavesTheSmsMessage()
        {
            var smsMessageDto = new SmsMessageDto();
            var contactInfo = new ContactInfo();
            var contactInfoObservable = _scheduler.CreateColdObservable(
                new Recorded<System.Reactive.Notification<ContactInfo>>(100, System.Reactive.Notification.CreateOnNext(contactInfo)));
            _mockContactRepository.Setup(cr => cr.GetOrCreateByPhoneNumber(It.IsAny<string>()))
                .Returns(contactInfoObservable);
            _mockContactRepository.Setup(cr => cr.UpdateLastActivityTime(contactInfo, null))
                .Returns(contactInfoObservable);

            _scheduler.Start(() => _subject.Create<RemoteSmsMessage>(smsMessageDto));

            _mockMessageRepository.Verify(cr => cr.Save(It.IsAny<RemoteSmsMessage>()), Times.Once);
        }

        [Test]
        public void Create_SaveSuccesful_ReturnsTheSavedMessage()
        {
            var smsMessageDto = new SmsMessageDto();
            var contactInfo = new ContactInfo();
            var contactInfoObservable = _scheduler.CreateColdObservable(new Recorded<System.Reactive.Notification<ContactInfo>>(100, System.Reactive.Notification.CreateOnNext(contactInfo)));
            _mockContactRepository.Setup(cr => cr.GetOrCreateByPhoneNumber(It.IsAny<string>())).Returns(contactInfoObservable);
            _mockContactRepository.Setup(cr => cr.UpdateLastActivityTime(contactInfo, null)).Returns(contactInfoObservable);
            var remoteSmsMessage = new RemoteSmsMessage();
            var smsMessageObservable = _scheduler.CreateColdObservable(new Recorded<System.Reactive.Notification<RepositoryOperation<RemoteSmsMessage>>>(100, System.Reactive.Notification.CreateOnNext(new RepositoryOperation<RemoteSmsMessage>(RepositoryMethodEnum.Changed, remoteSmsMessage))));
            _mockMessageRepository.Setup(mr => mr.Save(It.IsAny<RemoteSmsMessage>())).Returns(smsMessageObservable);

            var result = _scheduler.Start(() => _subject.Create<RemoteSmsMessage>(smsMessageDto));

            result.Messages.First().Value.Value.Should().Be(remoteSmsMessage);
        }
    }
}