namespace OmnipasteTests.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Contacts.Models;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Factories;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class ContactFactoryTests
    {
        private IContactFactory _subject;

        private Mock<IContactRepository> _mockContactRepository;

        [SetUp]
        public void SetUp()
        {
            _mockContactRepository = new Mock<IContactRepository> { DefaultValue = DefaultValue.Mock };
            _subject = new ContactFactory(_mockContactRepository.Object);
        }

        [Test]
        public void Create_Always_CallsGetByContactIdOrPhoneNumberWithTheCorrectParams()
        {
            var contactDto = new ContactDto
            {
                PhoneNumbers = new List<PhoneNumberDto> { new PhoneNumberDto { Number = "123" } },
                ContactId = 42
            };

            _subject.Create(contactDto).Subscribe();

            _mockContactRepository.Verify(m => m.GetByContactIdOrPhoneNumber(42, "123"), Times.Once());
        }

        [Test]
        public void Create_WhenLastActivityNull_UpdatesExistingContact()
        {
            using (TimeHelper.Freez())
            {
                var contactDto = new ContactDto
                {
                    LastName = "Ion",
                    PhoneNumbers = new List<PhoneNumberDto> { new PhoneNumberDto { Number = "123" } },
                    ContactId = 42
                };
                var contactInfo = new ContactInfo { UniqueId = "43", LastActivityTime = TimeHelper.UtcNow };
                _mockContactRepository.Setup(m => m.CreateIfNone(It.IsAny<IObservable<ContactInfo>>(), It.IsAny<Func<ContactInfo, ContactInfo>>())).Returns(Observable.Return(contactInfo));

                _subject.Create(contactDto).Subscribe();

                _mockContactRepository.Verify(m => m.Save(It.Is<ContactInfo>(ci => ci.UniqueId == "43" && ci.LastName == "Ion" && ci.LastActivityTime == TimeHelper.UtcNow)));
            }
        }

        [Test]
        public void Create_WhenLastActivity_DoesNotUpdatesExistingContact()
        {
            using (TimeHelper.Freez())
            {
                var contactDto = new ContactDto
                {
                    LastName = "Ion",
                    PhoneNumbers = new List<PhoneNumberDto> { new PhoneNumberDto { Number = "123" } }
                };
                var contactInfo = new ContactInfo { UniqueId = "43", LastName = "Gheo" };
                _mockContactRepository.Setup(m => m.CreateIfNone(It.IsAny<IObservable<ContactInfo>>(), It.IsAny<Func<ContactInfo, ContactInfo>>())).Returns(Observable.Return(contactInfo));

                _subject.Create(contactDto, TimeHelper.UtcNow).Subscribe();

                _mockContactRepository.Verify(m => m.Save(It.Is<ContactInfo>(ci => ci.UniqueId == "43" && ci.LastName == "Gheo" && ci.LastActivityTime == TimeHelper.UtcNow)));
            }
        }
    }
}