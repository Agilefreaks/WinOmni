namespace OmnipasteTests.Models
{
    using Events.Models;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Models;
    using Omnipaste.Models;

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
        public void Ctor_WithEvent_SetsPhoneToTheEventPhoneNumber()
        {
            var contactInfo = new ContactInfo(new Event { PhoneNumber = "somePhoneNumber" });

            contactInfo.Phone.Should().Be("somePhoneNumber");
        }

        [Test]
        public void Ctor_WithEventWithNoContactName_SetsEmptyStringForFirstNameAndLastName()
        {
            var contactInfo = new ContactInfo(new Event { ContactName = null });

            contactInfo.FirstName.Should().Be(string.Empty);
            contactInfo.LastName.Should().Be(string.Empty);
        }

        [Test]
        public void Ctor_WithEventWithContactNameMadeOfASingleWord_SetsTheContactNameAsTheFirstName()
        {
            var contactInfo = new ContactInfo(new Event { ContactName = "Word" });

            contactInfo.FirstName.Should().Be("Word");
        }

        [Test]
        public void Ctor_WithEventWithContactNameMadeOfTwoWords_SetsTheFirstWordAsTheFirstName()
        {
            var contactInfo = new ContactInfo(new Event { ContactName = "Word1 Word2" });

            contactInfo.FirstName.Should().Be("Word1");
        }

        [Test]
        public void Ctor_WithEventWithContactNameMadeOfTwoWords_SetsTheLastWordAsTheLastName()
        {
            var contactInfo = new ContactInfo(new Event { ContactName = "Word1 Word2" });

            contactInfo.LastName.Should().Be("Word2");
        }

        [Test]
        public void Ctor_WithEventWithContactNameMadeOfMoreThanTwoWords_SetsTheLastWordAsTheLastName()
        {
            var contactInfo = new ContactInfo(new Event { ContactName = "Word1 Word2 Word3" });

            contactInfo.LastName.Should().Be("Word3");
        }

        [Test]
        public void Ctor_WithEventWithContactNameMadeOfMoreThanTwoWords_SetsAllTheWordsExceptTheLastOneAsTheLastName()
        {
            var contactInfo = new ContactInfo(new Event { ContactName = "Word1 Word2 Word3 Word4" });

            contactInfo.FirstName.Should().Be("Word1 Word2 Word3");
        }

        [Test]
        public void Ctor_WithUserInfo_SetsFirstNameToTheUserInfoFirstName()
        {
            var contactInfo = new ContactInfo(new UserInfo { FirstName = "someName" });

            contactInfo.FirstName.Should().Be("someName");
        }

        [Test]
        public void Ctor_WithUserInfo_SetsLastNameToTheUserInfoLastName()
        {
            var contactInfo = new ContactInfo(new UserInfo { LastName = "someName" });

            contactInfo.LastName.Should().Be("someName");
        }

        [Test]
        public void Ctor_WithUserInfo_SetsImageUriToTheUserInfoImageUrl()
        {
            var contactInfo = new ContactInfo(new UserInfo { ImageUrl = "http://some_url/" });

            contactInfo.ImageUri.ToString().Should().Be("http://some_url/");
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