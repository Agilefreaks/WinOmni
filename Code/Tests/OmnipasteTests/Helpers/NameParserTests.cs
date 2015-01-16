namespace OmnipasteTests.Helpers
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Helpers;

    [TestFixture]
    public class NameParserTests
    {
        [Test]
        public void Parse_WhenNameIsEmpty_SetsEmptyStringForFirstName()
        {
            string firstName, lastName;
            NameParser.Parse(null, out firstName, out lastName);

            firstName.Should().BeEmpty();
        }

        [Test]
        public void Parse_WhenNameIsEmpty_SetsEmptyStringForLastName()
        {
            string firstName, lastName;
            NameParser.Parse(null, out firstName, out lastName);

            lastName.Should().BeEmpty();
        }

        [Test]
        public void Parse_WhenNameIsMadeOfASingleWord_SetsTheFirstNameToWord()
        {
            string firstName, lastName;
            NameParser.Parse("Word", out firstName, out lastName);

            firstName.Should().Be("Word");
        }

        [Test]
        public void Parse_WhenNameIsMadeOfASingleWord_SetsLastNameEmpty()
        {
            string firstName, lastName;
            NameParser.Parse("Word", out firstName, out lastName);

            lastName.Should().BeEmpty();
        }

        [Test]
        public void Parse_WhenNameIsMadeOfTwoWords_SetsTheFirstWordAsTheFirstName()
        {
            string firstName, lastName;
            NameParser.Parse("Word1 Word2", out firstName, out lastName);

            firstName.Should().Be("Word1");
        }

        [Test]
        public void Parse_WhenNameIsMadeOfTwoWords_SetsTheSecondWordAsTheLastName()
        {
            string firstName, lastName;
            NameParser.Parse("Word1 Word2", out firstName, out lastName);

            lastName.Should().Be("Word2");
        }

        [Test]
        public void Parse_WhenNameMadeOfMoreThanTwoWords_SetsTheFirstWordsAsTheLastName()
        {
            string firstName, lastName;
            NameParser.Parse("Word1 Word2 Word3", out firstName, out lastName);

            firstName.Should().Be("Word1 Word2");
        }

        [Test]
        public void Parse_WhenNameMadeOfMoreThanTwoWords_SetsTheLastWordAsTheLastName()
        {
            string firstName, lastName;
            NameParser.Parse("Word1 Word2 Word3", out firstName, out lastName);

            lastName.Should().Be("Word3");
        }
    }
}
