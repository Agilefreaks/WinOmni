namespace OmnipasteTests.Helpers
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Helpers;

    [TestFixture]
    public class TextParserTests
    {
        private TextParser _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new TextParser();
        }

        [Test]
        public void Parse_TextContainsHyperlinks_ReturnsAListOfTextPartsMarkedWithTheCorrectTextPartType()
        {
            var textParts = _subject.Parse("some plain text here\r\n http://hyperlink.com/test?asd=123 \r\n hello me");

            textParts.Count.Should().Be(3);
            textParts[0].Item1.Should().Be(TextPartTypeEnum.PlainText);
            textParts[0].Item2.Should().Be("some plain text here\r\n ");
            textParts[1].Item1.Should().Be(TextPartTypeEnum.Hyperlink);
            textParts[1].Item2.Should().Be("http://hyperlink.com/test?asd=123");
            textParts[2].Item1.Should().Be(TextPartTypeEnum.PlainText);
            textParts[2].Item2.Should().Be(" \r\n hello me");
        }

        [Test]
        public void Parse_TextEndsWithAHyperlink_ReturnsAListOfTextPartsMarkedWithTheCorrectTextPartType()
        {
            var textParts = _subject.Parse("some plain text here\r\n http://hyperlink.com/test?asd=123");

            textParts.Count.Should().Be(2);
            textParts[0].Item1.Should().Be(TextPartTypeEnum.PlainText);
            textParts[0].Item2.Should().Be("some plain text here\r\n ");
            textParts[1].Item1.Should().Be(TextPartTypeEnum.Hyperlink);
            textParts[1].Item2.Should().Be("http://hyperlink.com/test?asd=123");
        }

        [Test]
        public void Parse_TextHasOnlyAAHyperlink_ReturnsAListOfTextPartsMarkedWithTheCorrectTextPartType()
        {
            var textParts = _subject.Parse("http://hyperlink.com/test?asd=123");

            textParts.Count.Should().Be(1);
            textParts[0].Item1.Should().Be(TextPartTypeEnum.Hyperlink);
            textParts[0].Item2.Should().Be("http://hyperlink.com/test?asd=123");
        }

        [Test]
        public void Parse_TextDoesNotContainsHyperlinks_ReturnsAListOfTextPartsWithTheCorrectTextPartType()
        {
            var textParts = _subject.Parse("some plain text here\r\n hello me");

            textParts.Count.Should().Be(1);
            textParts[0].Item1.Should().Be(TextPartTypeEnum.PlainText);
            textParts[0].Item2.Should().Be("some plain text here\r\n hello me");
        }

        [Test]
        public void Parse_TextHasMoreThanOneHyperlink_ReturnsAListOfTextPartsMarkedWithTheCorrectTextPartType()
        {
            var textParts = _subject.Parse("some plain text here\r\n http://hyperlink.com/test?asd=123 some other here http://link2.com final text");

            textParts.Count.Should().Be(5);
            textParts[0].Item1.Should().Be(TextPartTypeEnum.PlainText);
            textParts[0].Item2.Should().Be("some plain text here\r\n ");
            textParts[1].Item1.Should().Be(TextPartTypeEnum.Hyperlink);
            textParts[1].Item2.Should().Be("http://hyperlink.com/test?asd=123");
            textParts[2].Item1.Should().Be(TextPartTypeEnum.PlainText);
            textParts[2].Item2.Should().Be(" some other here ");
            textParts[3].Item1.Should().Be(TextPartTypeEnum.Hyperlink);
            textParts[3].Item2.Should().Be("http://link2.com");
            textParts[4].Item1.Should().Be(TextPartTypeEnum.PlainText);
            textParts[4].Item2.Should().Be(" final text");
        }
    }
}