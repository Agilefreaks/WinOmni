namespace OmnipasteTests.Models
{
    using Events.Models;
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Models;

    [TestFixture]
    public class EventEventContactInfoTests
    {
        [Test]
        public void Ctor_WithEvent_SetsPhoneToTheEventPhoneNumber()
        {
            var EventContactInfo = new EventContactInfo(new Event { PhoneNumber = "somePhoneNumber" });

            EventContactInfo.Phone.Should().Be("somePhoneNumber");
        }

        [Test]
        public void Ctor_WithEventWithNoContactName_SetsEmptyStringForFirstNameAndLastName()
        {
            var EventContactInfo = new EventContactInfo(new Event { ContactName = null });

            EventContactInfo.FirstName.Should().Be(string.Empty);
            EventContactInfo.LastName.Should().Be(string.Empty);
        }

        [Test]
        public void Ctor_WithEventWithContactNameMadeOfASingleWord_SetsTheContactNameAsTheFirstName()
        {
            var EventContactInfo = new EventContactInfo(new Event { ContactName = "Word" });

            EventContactInfo.FirstName.Should().Be("Word");
        }

        [Test]
        public void Ctor_WithEventWithContactNameMadeOfTwoWords_SetsTheFirstWordAsTheFirstName()
        {
            var EventContactInfo = new EventContactInfo(new Event { ContactName = "Word1 Word2" });

            EventContactInfo.FirstName.Should().Be("Word1");
        }

        [Test]
        public void Ctor_WithEventWithContactNameMadeOfTwoWords_SetsTheLastWordAsTheLastName()
        {
            var EventContactInfo = new EventContactInfo(new Event { ContactName = "Word1 Word2" });

            EventContactInfo.LastName.Should().Be("Word2");
        }

        [Test]
        public void Ctor_WithEventWithContactNameMadeOfMoreThanTwoWords_SetsTheLastWordAsTheLastName()
        {
            var EventContactInfo = new EventContactInfo(new Event { ContactName = "Word1 Word2 Word3" });

            EventContactInfo.LastName.Should().Be("Word3");
        }

        [Test]
        public void Ctor_WithEventWithContactNameMadeOfMoreThanTwoWords_SetsAllTheWordsExceptTheLastOneAsTheLastName()
        {
            var EventContactInfo = new EventContactInfo(new Event { ContactName = "Word1 Word2 Word3 Word4" });

            EventContactInfo.FirstName.Should().Be("Word1 Word2 Word3");
        } 
    }
}