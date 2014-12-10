namespace OmnipasteTests.Framework.Converters
{
    using System.Windows;
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Framework.Converters;

    [TestFixture]
    public class StringNullOrEmptyToVisibilityConverterTests
    {
        private StringNullOrEmptyToVisibilityConverter _subject;

        [SetUp]
        public void SetUp()
        {
            _subject = new StringNullOrEmptyToVisibilityConverter();
        }

        [Test]
        public void Convert_WhenValueIsNull_ReturnsCollapsed()
        {
            object value = null;

            var result = _subject.Convert(value, null, null, null);

            result.Should().Be(Visibility.Collapsed);
        }

        [Test]
        public void Convert_WhenValueIsStringEmpty_ReturnsCollapsed()
        {
            object value = string.Empty;

            var result = _subject.Convert(value, null, null, null);

            result.Should().Be(Visibility.Collapsed);
        }

        [Test]
        public void Convert_WhenValueIsString_ReturnsVisible()
        {
            object value = "42";

            var result = _subject.Convert(value, null, null, null);

            result.Should().Be(Visibility.Visible);
        }
    }
}
