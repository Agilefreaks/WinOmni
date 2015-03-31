namespace OmnipasteTests.Models
{
    using System;
    using System.Collections.Generic;
    using Contacts.Dto;
    using Contacts.Models;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Entities;
    using Omnipaste.Models;

    [TestFixture]
    public class ContactInfoTests
    {
        private ContactEntity _subject;

        [SetUp]
        public void Setup()
        {
            TimeHelper.UtcNow = new DateTime(2015, 1, 1);
            _subject = new ContactEntity();
        }

        [TearDown]
        public void TearDown()
        {
            TimeHelper.Reset();
        }

        [Test]
        public void Ctor_NoArgumentsGiven_SetsFirstNameToEmptyString()
        {
            _subject.FirstName.Should().Be(string.Empty);
        }

        [Test]
        public void Ctor_NoArgumentsGiven_SetsLastNameToEmptyString()
        {
            _subject.LastName.Should().Be(string.Empty);
        }

        [Test]
        public void Ctor_Always_AssignsUniqueId()
        {
            _subject.UniqueId.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Ctor_Always_AssignsTime()
        {
            _subject.Time.Should().Be(TimeHelper.UtcNow.ToUniversalTime());
        }

        [Test]
        public void Name_Always_ReturnsTheFirstNameFollowedByASeparatorAndThenTheLastName()
        {
            _subject.FirstName = "some first name";
            _subject.LastName = "last";

            _subject.Name.Should().Be("some first name last");
        }

        [Test]
        public void ToString_Always_ReturnsTheNameAndPhone()
        {
            _subject.FirstName = "some first name";
            _subject.LastName = "some last name";
            _subject.PhoneNumbers = new[] { new PhoneNumber { Number = "12367" } };

            _subject.ToString().Should().Be("some first name some last name 12367");
        }

        [Test]
        public void Ctor_WithContact_SetsCorrectProperties()
        {
            var contact = new ContactDto
                              {
                                  ContactId = 884,
                                  FirstName = "first name",
                                  LastName = "last name",
                                  Image = "image base64 string"
                              };
            
            _subject = new ContactEntity(contact);

            _subject.ContactId.Should().Be(contact.ContactId);
            _subject.FirstName.Should().Be(contact.FirstName);
            _subject.LastName.Should().Be(contact.LastName);
            _subject.Image.Should().Be(contact.Image);
        }

        [Test]
        public void Ctor_NoArguments_HasEmptyListOfPhoneNumbers()
        {
            _subject.PhoneNumbers.Should().BeEmpty();
        }

        [Test]
        public void Ctor_WithContact_SetsCorrectPhoneNumbers()
        {
            var contactPhoneNumbers = new List<PhoneNumberDto>
                                          {
                                              new PhoneNumberDto { Number = "1234", Type = "Home" },
                                              new PhoneNumberDto { Number = "4567", Type = "Work" }
                                          };
            var contact = new ContactDto { PhoneNumbers = contactPhoneNumbers };
            
            _subject = new ContactEntity(contact);

            _subject.PhoneNumbers.Should().HaveCount(2);
            _subject.PhoneNumbers[0].Number.Should().Be("1234");
            _subject.PhoneNumbers[0].Type.Should().Be("Home");
            _subject.PhoneNumbers[1].Number.Should().Be("4567");
            _subject.PhoneNumbers[1].Type.Should().Be("Work");
        }

        [Test]
        public void PhoneNumber_WhenThereIsAPhoneNumber_ReturnsThePhoneNumber()
        {
            var contactInfo = new ContactEntity
                                  {
                                      PhoneNumbers =
                                          new List<PhoneNumber>
                                              {
                                                  new PhoneNumber
                                                      {
                                                          Number = "1234",
                                                          Type = "Work"
                                                      }
                                              }
                                  };

            contactInfo.PhoneNumber.Should().Be("1234");
        }

        [Test]
        public void PhoneNumber_WhenThereAreTwoPhoneNumbers_ReturnsTheFirstPhoneNumber()
        {
            var contactInfo = new ContactEntity
                                  {
                                      PhoneNumbers =
                                          new List<PhoneNumber>
                                              {
                                                  new PhoneNumber
                                                      {
                                                          Number = "1234",
                                                          Type = "Work"
                                                      },
                                                  new PhoneNumber()
                                                      {
                                                          Number = "5678",
                                                          Type = "Home"
                                                      }
                                              }
                                  };

            contactInfo.PhoneNumber.Should().Be("1234");
        }

        [Test]
        public void PhoneNumber_WhenThereIsNoNumber_ReturnsEmptyString()
        {
            var contactInfo = new ContactEntity();

            contactInfo.PhoneNumber.Should().Be(string.Empty);
        }
    }
}