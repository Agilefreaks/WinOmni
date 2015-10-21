namespace OmnipasteTests.Conversations.Conversation
{
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Conversations.Conversation;
    using Omnipaste.Framework.Models;

    [TestFixture]
    public class RecepientsTokenizerTests
    {
        private RecepientsTokenizer _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new RecepientsTokenizer();
        }

        [Test]
        public void Tokenize_AndTextDoesNotContainTheSeparator_WillReturnAResultWithZeroTokens()
        {
            var tokenizeResult = _subject.Tokenize("42");

            tokenizeResult.Tokens.Count().Should().Be(0);
        }

        [Test]
        public void Tokenize_AndTextDoesNotContainTheSeparator_WillReturnAResultWithTheEntireTextAsNonTokenizedText()
        {
            var tokenizeResult = _subject.Tokenize("42");

            tokenizeResult.NonTokenizedText.Should().Be("42");
        }

        [Test]
        public void Tokenize_AndTextContainsOnlyTheSeparator_WillReturnResultWithZeroTokens()
        {
            var tokenizeResult = _subject.Tokenize(RecepientsTokenizer.TokenSeparator);

            tokenizeResult.Tokens.Count().Should().Be(0);
        }

        [Test]
        public void Tokenize_AndTextContainsOnlyTheSeparator_WillReturnResultWithEmptyStringAsNonTokenizedText()
        {
            var tokenizeResult = _subject.Tokenize(RecepientsTokenizer.TokenSeparator);

            tokenizeResult.NonTokenizedText.Should().BeEmpty();
        }

        [Test]
        public void Tokenize_WithTextContainingRegularTextAndASeparator_WillReturnResultWithOneToken()
        {
            var tokenizeResult = _subject.Tokenize("42" + RecepientsTokenizer.TokenSeparator);

            tokenizeResult.Tokens.Count().Should().Be(1);
        }

        [Test]
        public void Tokenize_WithTextContainingRegularTextAndASeparator_WillReturnResultWithEmptyStringAsNonTokenizedText()
        {
            var tokenizeResult = _subject.Tokenize("42" + RecepientsTokenizer.TokenSeparator);

            tokenizeResult.NonTokenizedText.Should().BeEmpty();
        }

        [Test]
        public void Tokenize_WithStringWithMultipleSeparators_WillAddATokenForEachSeparatorInstance()
        {
            var text = string.Join(RecepientsTokenizer.TokenSeparator, "42", "43", "44")
                       + RecepientsTokenizer.TokenSeparator;

            var tokenizeResult = _subject.Tokenize(text);

            tokenizeResult.Tokens.Count().Should().Be(3);
            var tokens = tokenizeResult.Tokens.Cast<ContactModel>().ToList();
            tokens.First().PhoneNumber.Should().Be("42");
            tokens.ElementAt(1).PhoneNumber.Should().Be("43");
            tokens.Last().PhoneNumber.Should().Be("44");
        }
        
        [Test]
        public void Tokenize_WithStringWithMultipleSeparators_WillReturnResultWithEmptyStringAsNonTokenizedText()
        {
            var text = string.Join(RecepientsTokenizer.TokenSeparator, "42", "43", "44")
                       + RecepientsTokenizer.TokenSeparator;

            var tokenizeResult = _subject.Tokenize(text);

            tokenizeResult.Tokens.Count().Should().Be(3);
            tokenizeResult.NonTokenizedText.Should().BeEmpty();
        }

        [Test]
        public void Tokenize_WithAnElementAppearingMultipleTimes_WillReturnsAResultContainingUniqueTokens()
        {
            var result = _subject.Tokenize(string.Join(RecepientsTokenizer.TokenSeparator, "42", "42") + RecepientsTokenizer.TokenSeparator);

            result.Tokens.Count().Should().Be(1);
        }

        [Test]
        public void Tokenize_WithAnElementAppearingMultipleTimes_WillReturnResultWithEmptyStringAsNonTokenizedText()
        {
            var result = _subject.Tokenize(string.Join(RecepientsTokenizer.TokenSeparator, "42", "42") + RecepientsTokenizer.TokenSeparator);

            result.NonTokenizedText.Should().BeEmpty();
        }
    }
}