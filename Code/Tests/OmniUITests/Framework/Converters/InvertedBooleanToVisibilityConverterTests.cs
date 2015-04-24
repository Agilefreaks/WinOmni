namespace OmniUITests.Framework.Converters
{
    using System.Windows;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniUI.Framework.Converters;

    [TestFixture]
    public class InvertedBooleanToVisibilityConverterTests
    {
        private InvertedBooleanToVisibilityConverter _subject;

        [SetUp]
        public void SetUp()
        {
            _subject = new InvertedBooleanToVisibilityConverter();
        }

        [Test]
        public void Convert_WhenValueIsTrue_ReturnsCollapsed()
        {
            var result = _subject.Convert(true, null, null, null);

            result.Should().Be(Visibility.Collapsed);
        }

        [Test]
        public void Convert_WhenValueIsFalse_ReturnsVisible()
        {
            var result = _subject.Convert(false, null, null, null);

            result.Should().Be(Visibility.Visible);
        }
    }
}
