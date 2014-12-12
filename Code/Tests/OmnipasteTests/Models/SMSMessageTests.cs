namespace OmnipasteTests.Models
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Models;

    [TestFixture]
    public class SMSMessageTests
    {
        private SMSMessage _subject;

        [SetUp]
        public void SetUp()
        {
            _subject = new SMSMessage(new Message());
        }

        [Test]
        public void CharactersRemaining_WhenMessageLengthIsZero_Returns160()
        {
            _subject.CharactersRemaining.Should().Be(160);
        }

        [Test]
        public void CharactersRemaining_WhenMessageLengthIs160_Returns0()
        {
            _subject.Message = new string('x', 160);

            _subject.CharactersRemaining.Should().Be(0);
        }

        [Test]
        public void CharactersRemaining_WhenMessageLengthIsMultipleOf160_Returns0()
        {
            _subject.Message = new string('x', 320);

            _subject.CharactersRemaining.Should().Be(0);
        }

        [Test]
        public void CharactersRemaining_WhenMessageLengthIs1_Returns159()
        {
            _subject.Message = "x";

            _subject.CharactersRemaining.Should().Be(159);
        }

        [Test]
        public void CharactersRemaining_WhenMessageLengthIs161_Returns159()
        {
            _subject.Message = new string('x', 161);

            _subject.CharactersRemaining.Should().Be(159);
        }

        [Test]
        public void MaxCharacters_WhenLengthIs0_Returns160()
        {
            _subject.MaxCharacters.Should().Be(160);
        }

        [Test]
        public void MaxCharacters_WhenLengthIs1_Returns160()
        {
            _subject.Message = "x";

            _subject.MaxCharacters.Should().Be(160);
        }

        [Test]
        public void MaxCharacters_WhenLengthIs160_Returns160()
        {
            _subject.Message = new string('x', 160);

            _subject.MaxCharacters.Should().Be(160);
        }

        [Test]
        public void MaxCharacters_WhenLengthIs161_Returns320()
        {
            _subject.Message = new string('x', 161);

            _subject.MaxCharacters.Should().Be(320);
        }
    }
}