namespace OmnipasteTests.Framework.Converters
{
    using System;
    using System.Globalization;
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Framework.Converters;

    [TestFixture]
    public class HumanizedStringConverterTests
    {
        private HumanizedStringConverter _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new HumanizedStringConverter();
        }

        [Test]
        public void Convert_ValueIsCamelCaseString_ReturnsHumanizedString()
        {
            _subject.Convert("SomeCamelCaseString", typeof(string), null, CultureInfo.InvariantCulture)
                .Should()
                .Be("Some camel case string");
        }

        [Test]
        public void Convert_ValueIsUnderscoredString_ReturnsHumanizedString()
        {
            _subject.Convert("Some_underscored_string", typeof(string), null, CultureInfo.InvariantCulture)
                .Should()
                .Be("Some underscored string");
        }

        [Test]
        public void Convert_ValueIsEnumWithDescription_ReturnsDescriptionOfValue()
        {
            _subject.Convert(SomeEnum.ValueWithDescription, typeof(string), null, CultureInfo.InvariantCulture)
                .Should()
                .Be("Some description");
        }

        [Test]
        public void Convert_ValueIsEnumWithoutDescription_ReturnsValueAsHumanizedString()
        {
            _subject.Convert(SomeEnum.ValueWithoutDescription, typeof(string), null, CultureInfo.InvariantCulture)
                .Should()
                .Be("Value without description");
        }

        [Test]
        public void Convert_ValueIsCurrentDateTime_ReturnsApproximateTimeAgo()
        {
            var dateTime = DateTime.UtcNow - TimeSpan.FromDays(5);
            _subject.Convert(dateTime, typeof(string), null, CultureInfo.InvariantCulture)
                .Should()
                .Be("5 days ago");
        }

        private enum SomeEnum
        {
            [System.ComponentModel.Description("Some description")]
            ValueWithDescription,
            ValueWithoutDescription
        }
    }
}