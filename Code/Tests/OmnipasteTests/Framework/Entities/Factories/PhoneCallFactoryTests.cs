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
    using PhoneCalls.Dto;

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

                _scheduler.Start(() => _subject.Create<LocalPhoneCallEntity>(phoneCallDto));

                _mockContactFactory.Verify(cr => cr.Create(It.Is<ContactDto>(c => c.ContactId == 42 && c.PhoneNumbers.Any(pn => pn.Number == "123")), TimeHelper.UtcNow), Times.Once);                
            }
        }

        [Test]
        public void Create_Always_SavesPhoneCall()
        {
            var phoneCallDto = new PhoneCallDto();
            var contactEntity = new ContactEntity();
            var contactObservable = _scheduler.CreateColdObservable(
                new Recorded<Notification<ContactEntity>>(100, Notification.CreateOnNext(contactEntity)));
            _mockContactFactory.Setup(m => m.Create(It.IsAny<ContactDto>(), It.IsAny<DateTime>())).Returns(contactObservable);
            _scheduler.Start(() => _subject.Create<LocalPhoneCallEntity>(phoneCallDto));

            _mockPhoneCallRepository.Verify(pcr => pcr.Save(It.IsAny<LocalPhoneCallEntity>()), Times.Once);
        }

        [Test]
        public void Create_SaveSuccesful_ReturnsTheSavedPhoneCall()
        {
            var phoneCallDto = new PhoneCallDto();
            var contactEntity = new ContactEntity();
            var contactObservable = _scheduler.CreateColdObservable(new Recorded<Notification<ContactEntity>>(100, Notification.CreateOnNext(contactEntity)));
            _mockContactFactory.Setup(m => m.Create(It.IsAny<ContactDto>(), It.IsAny<DateTime>())).Returns(contactObservable);
            var phoneCall = new LocalPhoneCallEntity();
            var phoneCallObservable = _scheduler.CreateColdObservable(new Recorded<Notification<RepositoryOperation<LocalPhoneCallEntity>>>(100, Notification.CreateOnNext(new RepositoryOperation<LocalPhoneCallEntity>(RepositoryMethodEnum.Changed, phoneCall))));
            _mockPhoneCallRepository.Setup(pcr => pcr.Save(It.IsAny<LocalPhoneCallEntity>())).Returns(phoneCallObservable);

            var result = _scheduler.Start(() => _subject.Create<LocalPhoneCallEntity>(phoneCallDto));

            result.Messages.First().Value.Value.Should().Be(phoneCall);
        }
    }
}