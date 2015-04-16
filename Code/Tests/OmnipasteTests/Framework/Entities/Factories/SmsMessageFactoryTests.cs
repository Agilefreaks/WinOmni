namespace OmnipasteTests.Framework.Entities.Factories
{
    using System;
    using System.Linq;
    using System.Reactive;
    using Contacts.Dto;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Entities.Factories;
    using Omnipaste.Framework.Services.Repositories;
    using SMS.Dto;

    [TestFixture]
    public class SmsMessageFactoryTests
    {
        private MoqMockingKernel _kernel;

        private ISmsMessageFactory _subject;

        private TestScheduler _scheduler;

        private Mock<ISmsMessageRepository> _mockMessageRepository;

        private Mock<IContactFactory> _mockContactFactory;
        
        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _mockMessageRepository = new Mock<ISmsMessageRepository> { DefaultValue = DefaultValue.Mock };
            _mockContactFactory = new Mock<IContactFactory> { DefaultValue = DefaultValue.Mock };

            _kernel.Bind<ISmsMessageRepository>().ToConstant(_mockMessageRepository.Object);
            _kernel.Bind<IContactFactory>().ToConstant(_mockContactFactory.Object);

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
        public void Create_AlwaysCalls_CreateOnContactFactory()
        {
            using (TimeHelper.Freez())
            {
                var smsMessageDto = new SmsMessageDto { PhoneNumber = "42" };

                _scheduler.Start(() => _subject.Create<RemoteSmsMessageEntity>(smsMessageDto));

                _mockContactFactory.Verify(cr => cr.Create(It.Is<ContactDto>(c => c.PhoneNumbers.Any(pn => pn.Number == "42")), TimeHelper.UtcNow), Times.Once);                
            }
        }

        [Test]
        public void Create_Always_SavesTheSmsMessage()
        {
            var smsMessageDto = new SmsMessageDto { PhoneNumber = "42" };
            var contactEntity = new ContactEntity();
            var contactObservable = _scheduler.CreateColdObservable(new Recorded<Notification<ContactEntity>>(100, Notification.CreateOnNext(contactEntity)));
            _mockContactFactory.Setup(cr => cr.Create(It.IsAny<ContactDto>(), It.IsAny<DateTime>())).Returns(contactObservable); 

            _scheduler.Start(() => _subject.Create<RemoteSmsMessageEntity>(smsMessageDto));

            _mockMessageRepository.Verify(cr => cr.Save(It.IsAny<RemoteSmsMessageEntity>()), Times.Once);
        }

        [Test]
        public void Create_SaveSuccesful_ReturnsTheSavedMessage()
        {
            var smsMessageDto = new SmsMessageDto { PhoneNumber = "42" };
            var contactEntity = new ContactEntity();
            var contactObservable = _scheduler.CreateColdObservable(new Recorded<Notification<ContactEntity>>(100, Notification.CreateOnNext(contactEntity)));
            _mockContactFactory.Setup(cr => cr.Create(It.IsAny<ContactDto>(), It.IsAny<DateTime>())).Returns(contactObservable);
            var remoteSmsMessage = new RemoteSmsMessageEntity();
            var smsMessageObservable = _scheduler.CreateColdObservable(new Recorded<Notification<RepositoryOperation<RemoteSmsMessageEntity>>>(100, Notification.CreateOnNext(new RepositoryOperation<RemoteSmsMessageEntity>(RepositoryMethodEnum.Changed, remoteSmsMessage))));
            _mockMessageRepository.Setup(mr => mr.Save(It.IsAny<RemoteSmsMessageEntity>())).Returns(smsMessageObservable);

            var result = _scheduler.Start(() => _subject.Create<RemoteSmsMessageEntity>(smsMessageDto));

            result.Messages.First().Value.Value.Should().Be(remoteSmsMessage);
        }
    }
}