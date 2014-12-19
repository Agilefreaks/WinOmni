namespace OmnipasteTests.Models
{
    using FluentAssertions;
    using NUnit.Framework;
    using OmniUI.Models;

    [TestFixture]
    public class ContactInfoTests
    {
        private ContactInfo _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new ContactInfo();
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
        public void Name_Always_ReturnsTheFirstNameFollowedByASeparatorAndThenTheLastName()
        {
            _subject.FirstName = "some first name";
            _subject.LastName = "last";

            _subject.Name.Should().Be("some first name last");
        }
    }
}