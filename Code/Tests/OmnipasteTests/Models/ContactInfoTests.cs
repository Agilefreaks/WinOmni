namespace OmnipasteTests.Models
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Models;

    [TestFixture]
    public class ContactInfoTests
    {
        private ContactInfo _subject;

        [SetUp]
        public void Setup()
        {
            TimeHelper.UtcNow = new DateTime(2015, 1, 1);
            _subject = new ContactInfo();
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
        public void Ctor_NoArgumentsGiven_SetsPhoneToEmptyString()
        {
            _subject.Phone.Should().Be(string.Empty);
        }

        [Test]
        public void Ctor_Always_AssignsUniqueId()
        {
            _subject.UniqueId.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Ctor_Always_AssignsTime()
        {
            _subject.Time.Should().Be(TimeHelper.UtcNow);
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
            _subject.Phone = "12367";

            _subject.ToString().Should().Be("some first name some last name 12367");
        }
    }
}