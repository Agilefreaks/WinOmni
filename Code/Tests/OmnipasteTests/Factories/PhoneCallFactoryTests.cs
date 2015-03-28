namespace OmnipasteTests.Factories
{
    using System;
    using System.Linq;
    using System.Reactive;
    using Contacts.Models;
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
    using PhoneCalls.Models;

    [TestFixture]
    public class PhoneCallFactoryTests
    {
        private MoqMockingKernel _kernel;

        private IPhoneCallFactory _subject;

        private Mock<IContactFactory> _mockContactFactory;

        private Mock<IPhoneCallRepository> _mockPhoneCallRepository;

        private TestScheduler _scheduler;

        [SetUp]
        public void SetUp()
        {
            _mockPhoneCallRepository = new Mock<IPhoneCallRepository> { DefaultValue = DefaultValue.Mock };
            _mockContactFactory = new Mock<IContactFactory> { DefaultValue = DefaultValue.Mock };

            _kernel = new MoqMockingKernel();
            _kernel.Bind<IPhoneCallFactory>().To<PhoneCallFactory>();
            _kernel.Bind<IPhoneCallRepository>().ToConstant(_mockPhoneCallRepository.Object);
            _kernel.Bind<IContactFactory>().ToConstant(_mockContactFactory.Object);

            _subject = _kernel.Get<IPhoneCallFactory>();

            _scheduler = new TestScheduler();
            SchedulerProvider.Default = _scheduler;
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void Create_Always_CallsCreateWithCorrectParamss()
        {
            using (TimeHelper.Freez())
            {
                var phoneCallDto = new PhoneCallDto { Number = "123", ContactId = 42 };

                _scheduler.Start(() => _subject.Create<LocalPhoneCall>(phoneCallDto));

                _mockContactFactory.Verify(cr => cr.Create(It.Is<ContactDto>(c => c.ContactId == 42 && c.PhoneNumbers.Any(pn => pn.Number == "123")), TimeHelper.UtcNow), Times.Once);                
            }
        }

        [Test]
        public void Create_Always_SavesPhoneCall()
        {
            var phoneCallDto = new PhoneCallDto();
            var contactInfo = new ContactInfo();
            var contactInfoObservable = _scheduler.CreateColdObservable(
                new Recorded<Notification<ContactInfo>>(100, Notification.CreateOnNext(contactInfo)));
            _mockContactFactory.Setup(m => m.Create(It.IsAny<ContactDto>(), It.IsAny<DateTime>())).Returns(contactInfoObservable);
            _scheduler.Start(() => _subject.Create<LocalPhoneCall>(phoneCallDto));

            _mockPhoneCallRepository.Verify(pcr => pcr.Save(It.IsAny<LocalPhoneCall>()), Times.Once);
        }

        [Test]
        public void Create_SaveSuccesful_ReturnsTheSavedPhoneCall()
        {
            var phoneCallDto = new PhoneCallDto();
            var contactInfo = new ContactInfo();
            var contactInfoObservable = _scheduler.CreateColdObservable(new Recorded<Notification<ContactInfo>>(100, Notification.CreateOnNext(contactInfo)));
            _mockContactFactory.Setup(m => m.Create(It.IsAny<ContactDto>(), It.IsAny<DateTime>())).Returns(contactInfoObservable);
            var phoneCall = new LocalPhoneCall();
            var phoneCallObservable = _scheduler.CreateColdObservable(new Recorded<Notification<RepositoryOperation<LocalPhoneCall>>>(100, Notification.CreateOnNext(new RepositoryOperation<LocalPhoneCall>(RepositoryMethodEnum.Changed, phoneCall))));
            _mockPhoneCallRepository.Setup(pcr => pcr.Save(It.IsAny<LocalPhoneCall>())).Returns(phoneCallObservable);

            var result = _scheduler.Start(() => _subject.Create<LocalPhoneCall>(phoneCallDto));

            result.Messages.First().Value.Value.Should().Be(phoneCall);
        }
    }
}