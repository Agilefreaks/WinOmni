namespace OmnipasteTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using Clipboard.Handlers;
    using Clipboard.Models;
    using Contacts.Handlers;
    using Contacts.Models;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Factories;
    using Omnipaste.Models;
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;
    using PhoneCalls.Handlers;
    using SMS.Handlers;

    [TestFixture]
    public class EntitySupervisorTests
    {
        private Mock<IPhoneCallRepository> _mockCallRepository;

        private Mock<IClipboardHandler> _mockClipboardHandler;

        private Mock<IClippingRepository> _mockClippingRepository;

        private Mock<IContactCreatedHandler> _mockContactCreatedHandler;

        private Mock<IContactRepository> _mockContactRepository;

        private Mock<IContactsUpdatedHandler> _mockContactsUpdatedHandler;
        
        private Mock<IPhoneCallReceivedHandler> _mockPhoneCallReceivedHandler;

        private Mock<ISmsMessageCreatedHandler> _mockSmsMessageCreatedHandler;

        private Mock<IUpdateInfoRepository> _mockUpdateInfoRepository;

        private Mock<IUpdaterService> _mockUpdaterService;

        private EntitySupervisor _subject;

        private TestScheduler _testScheduler;

        private Mock<IRemoteSmsMessageFactory> _mockRemoteSmsMessageFacotry;

        private Mock<IPhoneCallFactory> _mockPhoneCallFactory;

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
            _mockCallRepository = new Mock<IPhoneCallRepository> { DefaultValue = DefaultValue.Mock };
            _mockUpdaterService = new Mock<IUpdaterService> { DefaultValue = DefaultValue.Mock };
            _mockUpdateInfoRepository = new Mock<IUpdateInfoRepository> { DefaultValue = DefaultValue.Mock };
            _mockContactRepository = new Mock<IContactRepository> { DefaultValue = DefaultValue.Mock };
            _mockContactsUpdatedHandler = new Mock<IContactsUpdatedHandler> { DefaultValue = DefaultValue.Mock };
            _mockRemoteSmsMessageFacotry = new Mock<IRemoteSmsMessageFactory> { DefaultValue = DefaultValue.Mock };
            _mockPhoneCallFactory = new Mock<IPhoneCallFactory> { DefaultValue = DefaultValue.Mock };

            _subject = new EntitySupervisor
                           {
                               ClipboardHandler = _mockClipboardHandler.Object,
                               ClippingRepository = _mockClippingRepository.Object,
                               PhoneCallReceivedHandler = _mockPhoneCallReceivedHandler.Object,
                               SmsMessageCreatedHandler = _mockSmsMessageCreatedHandler.Object,
                               ContactCreatedHandler = _mockContactCreatedHandler.Object,
                               UpdaterService = _mockUpdaterService.Object,
                               UpdateInfoRepository = _mockUpdateInfoRepository.Object,
                               ContactRepository = _mockContactRepository.Object,
                               ContactsUpdatedHandler = _mockContactsUpdatedHandler.Object,
                               RemoteSmsMessageFactory = _mockRemoteSmsMessageFacotry.Object,
                               PhoneCallFactory = _mockPhoneCallFactory.Object
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
            var clipping = new Clipping { Id = Id };
            var clippingObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<Clipping>>(100, Notification.CreateOnNext(clipping)));
            _mockClipboardHandler.Setup(m => m.Subscribe(It.IsAny<IObserver<Clipping>>()))
                .Returns<IObserver<Clipping>>(o => clippingObservable.Subscribe(o));

            _subject.Start();
            _testScheduler.Start(() => clippingObservable);

            _mockClippingRepository.Verify(m => m.Save(It.Is<ClippingModel>(c => c.Id == Id)));
        }

        [Test]
        public void OnNewUpdateInfo_Always_SotresUpdateInfo()
        {
            const string UniqueId = "42";
            var updateInfo = new UpdateInfo { UniqueId = UniqueId };
            var updateInfoObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<UpdateInfo>>(100, Notification.CreateOnNext(updateInfo)));
            _mockUpdaterService.SetupGet(m => m.UpdateObservable).Returns(updateInfoObservable);

            _subject.Start();
            _testScheduler.Start(() => updateInfoObservable);

            _mockUpdateInfoRepository.Verify(m => m.Save(It.Is<UpdateInfo>(c => c.UniqueId == UniqueId)));
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

            _mockContactRepository.Verify(m => m.Save(It.IsAny<ContactInfo>()));
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

            _mockContactRepository.Verify(m => m.Save(It.IsAny<ContactInfo>()), Times.Exactly(2));
        }
    }
}