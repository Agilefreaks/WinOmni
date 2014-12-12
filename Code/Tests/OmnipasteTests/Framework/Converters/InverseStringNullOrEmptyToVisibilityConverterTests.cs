namespace OmnipasteTests.Framework.Converters
{
    using System.Windows;
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Framework.Converters;

    [TestFixture]
    public class InverseStringNullOrEmptyToVisibilityConverterTests
    {
        private InverseStringNullOrEmptyToVisibilityConverter _subject;

        [SetUp]
        public void SetUp()
        {
            _subject = new InverseStringNullOrEmptyToVisibilityConverter();
        }

        [Test]
        public void Convert_WhenValueIsNull_ReturnsVisible()
        {
            object value = null;

            var result = _subject.Convert(value, null, null, null);

            result.Should().Be(Visibility.Visible);
        }

        [Test]
        public void Convert_WhenValueIsStringEmpty_ReturnsVisible()
        {
            object value = string.Empty;

            var result = _subject.Convert(value, null, null, null);

            result.Should().Be(Visibility.Visible);
        }

        [Test]
        public void Convert_WhenValueIsString_ReturnsCollapsed()
        {
            object value = "42";

            var result = _subject.Convert(value, null, null, null);

            result.Should().Be(Visibility.Collapsed);
        }
    }
}