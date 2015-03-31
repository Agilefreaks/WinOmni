namespace OmnipasteTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using Clipboard.Dto;
    using Clipboard.Handlers;
    using Contacts.Dto;
    using Contacts.Handlers;
    using Contacts.Models;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Entities;
    using Omnipaste.Factories;
    using Omnipaste.Models;
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;
    using PhoneCalls.Handlers;
    using SMS.Handlers;

    [TestFixture]
    public class EntitySupervisorTests
    {
        private Mock<IClipboardHandler> _mockClipboardHandler;

        private Mock<IClippingRepository> _mockClippingRepository;

        private Mock<IContactCreatedHandler> _mockContactCreatedHandler;

        private Mock<IContactsUpdatedHandler> _mockContactsUpdatedHandler;
        
        private Mock<IPhoneCallReceivedHandler> _mockPhoneCallReceivedHandler;

        private Mock<ISmsMessageCreatedHandler> _mockSmsMessageCreatedHandler;

        private Mock<IUpdateInfoRepository> _mockUpdateInfoRepository;

        private Mock<IUpdaterService> _mockUpdaterService;

        private EntitySupervisor _subject;

        private TestScheduler _testScheduler;

        private Mock<ISmsMessageFactory> _mockSmsMessageFactory;

        private Mock<IPhoneCallFactory> _mockPhoneCallFactory;

        private Mock<IContactFactory> _mockContactFactory;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;

            _mockClipboardHandler = new Mock<IClipboardHandler> { DefaultValue = DefaultValue.Mock };
            _mockClippingRepository = new Mock<IClippingRepository> { DefaultValue = DefaultValue.Mock };
            _mockPhoneCallReceivedHandler = new Mock<IPhoneCallReceivedHandler> { DefaultValue = DefaultValue.Mock };
            _mockSmsMessageCreatedHandler = new Mock<ISmsMessageCreatedHandler> { DefaultValue = DefaultValue.Mock };
            _mockContactCreatedHandler = new Mock<IContactCreatedHandler> { DefaultValue = DefaultValue.Mock };
            _mockUpdaterService = new Mock<IUpdaterService> { DefaultValue = DefaultValue.Mock };
            _mockUpdateInfoRepository = new Mock<IUpdateInfoRepository> { DefaultValue = DefaultValue.Mock };
            _mockContactsUpdatedHandler = new Mock<IContactsUpdatedHandler> { DefaultValue = DefaultValue.Mock };
            _mockSmsMessageFactory = new Mock<ISmsMessageFactory> { DefaultValue = DefaultValue.Mock };
            _mockPhoneCallFactory = new Mock<IPhoneCallFactory> { DefaultValue = DefaultValue.Mock };
            _mockContactFactory = new Mock<IContactFactory> { DefaultValue = DefaultValue.Mock };

            _subject = new EntitySupervisor
                           {
                               ClipboardHandler = _mockClipboardHandler.Object,
                               ClippingRepository = _mockClippingRepository.Object,
                               UpdaterService = _mockUpdaterService.Object,
                               UpdateInfoRepository = _mockUpdateInfoRepository.Object,
                               PhoneCallReceivedHandler = _mockPhoneCallReceivedHandler.Object,
                               SmsMessageCreatedHandler = _mockSmsMessageCreatedHandler.Object,
                               ContactCreatedHandler = _mockContactCreatedHandler.Object,
                               ContactsUpdatedHandler = _mockContactsUpdatedHandler.Object,
                               SmsMessageFactory = _mockSmsMessageFactory.Object,
                               PhoneCallFactory = _mockPhoneCallFactory.Object,
                               ContactFactory = _mockContactFactory.Object
                           };
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void OnNewClipping_AfterStart_AlwayStoresClipping()
        {
            const string Id = "42";
            var clipping = new ClippingDto { Id = Id };
            var clippingObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<ClippingDto>>(100, Notification.CreateOnNext(clipping)));
            _mockClipboardHandler.Setup(m => m.Subscribe(It.IsAny<IObserver<ClippingDto>>()))
                .Returns<IObserver<ClippingDto>>(o => clippingObservable.Subscribe(o));

            _subject.Start();
            _testScheduler.Start(() => clippingObservable);

            _mockClippingRepository.Verify(m => m.Save(It.Is<ClippingEntity>(c => c.Id == Id)));
        }

        [Test]
        public void OnNewUpdateInfo_Always_SotresUpdateInfo()
        {
            const string UniqueId = "42";
            var updateInfo = new UpdateEntity { UniqueId = UniqueId };
            var updateInfoObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<UpdateEntity>>(100, Notification.CreateOnNext(updateInfo)));
            _mockUpdaterService.SetupGet(m => m.UpdateObservable).Returns(updateInfoObservable);

            _subject.Start();
            _testScheduler.Start(() => updateInfoObservable);

            _mockUpdateInfoRepository.Verify(m => m.Save(It.Is<UpdateEntity>(c => c.UniqueId == UniqueId)));
        }

        [Test]
        public void OnNewContactInfo_Always_SavesTheContact()
        {
            var contact = new ContactDto { FirstName = "first name" };
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<ContactDto>>(100, Notification.CreateOnNext(contact)));
            _mockContactCreatedHandler.Setup(m => m.Subscribe(It.IsAny<IObserver<ContactDto>>()))
                .Returns<IObserver<ContactDto>>(o => contactObservable.Subscribe(o));

            _subject.Start();
            _testScheduler.Start(() => contactObservable);

            _mockContactFactory.Verify(m => m.Create(It.Is<ContactDto>(c => c == contact)));
        }

        [Test]
        public void OnContactsUpdated_Always_SavesAllTheContacts()
        {
            var contactList = new List<ContactDto> { new ContactDto(), new ContactDto() };
            var contactListObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<List<ContactDto>>>(100, Notification.CreateOnNext(contactList)));
            _mockContactsUpdatedHandler.Setup(m => m.Subscribe(It.IsAny<IObserver<List<ContactDto>>>()))
                .Returns<IObserver<List<ContactDto>>>(o => contactListObservable.Subscribe(o));

            _subject.Start();
            _testScheduler.Start(() => contactListObservable);

            _mockContactFactory.Verify(m => m.Create(It.IsAny<ContactDto>()), Times.Exactly(2));
        }
    }
}