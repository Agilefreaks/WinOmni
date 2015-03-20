namespace OmnipasteTests.Factories
{
    using System.Linq;
    using System.Reactive;
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

        private Mock<IContactRepository> _mockContactRepository;

        private Mock<IPhoneCallRepository> _mockPhoneCallRepository;

        private TestScheduler _scheduler;

        [SetUp]
        public void SetUp()
        {
            _mockPhoneCallRepository = new Mock<IPhoneCallRepository> { DefaultValue = DefaultValue.Mock };
            _mockContactRepository = new Mock<IContactRepository> { DefaultValue = DefaultValue.Mock };

            _kernel = new MoqMockingKernel();
            _kernel.Bind<IPhoneCallFactory>().To<PhoneCallFactory>();
            _kernel.Bind<IPhoneCallRepository>().ToConstant(_mockPhoneCallRepository.Object);
            _kernel.Bind<IContactRepository>().ToConstant(_mockContactRepository.Object);

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
        public void Create_Always_CallsGetOrCreateByPhoneNumberContact()
        {
            var phoneCallDto = new PhoneCallDto();

            _scheduler.Start(() => _subject.Create<LocalPhoneCall>(phoneCallDto));

            _mockContactRepository.Verify(cr => cr.GetOrCreateByPhoneNumber(phoneCallDto.Number), Times.Once);
        }

        [Test]
        public void Create_Always_UpdatesLastActivityTimeOnTheContact()
        {
            var phoneCallDto = new PhoneCallDto { Number = "42" };
            var contactInfo = new ContactInfo();
            var contactInfoObservable = _scheduler.CreateColdObservable(
                new Recorded<Notification<ContactInfo>>(100, Notification.CreateOnNext(contactInfo)));
            _mockContactRepository.Setup(m => m.GetOrCreateByPhoneNumber("42")).Returns(contactInfoObservable);

            _scheduler.Start(() => _subject.Create<LocalPhoneCall>(phoneCallDto));

            _mockContactRepository.Verify(cr => cr.UpdateLastActivityTime(contactInfo, null), Times.Once);
        }

        [Test]
        public void Create_Always_SavesPhoneCall()
        {
            var phoneCallDto = new PhoneCallDto();
            var contactInfo = new ContactInfo();
            var contactInfoObservable = _scheduler.CreateColdObservable(
                new Recorded<Notification<ContactInfo>>(100, Notification.CreateOnNext(contactInfo)));
            _mockContactRepository.Setup(cr => cr.GetOrCreateByPhoneNumber(It.IsAny<string>()))
                .Returns(contactInfoObservable);
            _mockContactRepository.Setup(cr => cr.UpdateLastActivityTime(contactInfo, null))
                .Returns(contactInfoObservable);
            _scheduler.Start(() => _subject.Create<LocalPhoneCall>(phoneCallDto));

            _mockPhoneCallRepository.Verify(pcr => pcr.Save(It.IsAny<LocalPhoneCall>()), Times.Once);
        }

        [Test]
        public void Create_SaveSuccesful_ReturnsTheSavedPhoneCall()
        {
            var phoneCallDto = new PhoneCallDto();
            var contactInfo = new ContactInfo();
            var contactInfoObservable = _scheduler.CreateColdObservable(new Recorded<Notification<ContactInfo>>(100, Notification.CreateOnNext(contactInfo)));
            _mockContactRepository.Setup(cr => cr.GetOrCreateByPhoneNumber(It.IsAny<string>())).Returns(contactInfoObservable);
            _mockContactRepository.Setup(cr => cr.UpdateLastActivityTime(contactInfo, null)).Returns(contactInfoObservable);
            var phoneCall = new LocalPhoneCall();
            var phoneCallObservable = _scheduler.CreateColdObservable(new Recorded<Notification<RepositoryOperation<LocalPhoneCall>>>(100, Notification.CreateOnNext(new RepositoryOperation<LocalPhoneCall>(RepositoryMethodEnum.Changed, phoneCall))));
            _mockPhoneCallRepository.Setup(pcr => pcr.Save(It.IsAny<LocalPhoneCall>())).Returns(phoneCallObservable);

            var result = _scheduler.Start(() => _subject.Create<LocalPhoneCall>(phoneCallDto));

            result.Messages.First().Value.Value.Should().Be(phoneCall);
        }
    }
}