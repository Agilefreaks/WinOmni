namespace OmnipasteTests.Framework.Converters
{
    using System.Windows;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniUI.Converters;

    [TestFixture]
    public class InverseBooleanToVisibilityConverterTests
    {
        private InverseBooleanToVisibilityConverter _subject;

        [SetUp]
        public void SetUp()
        {
            _subject = new InverseBooleanToVisibilityConverter();
        }

        [Test]
        public void Convert_WhenValueIsTrue_ReturnsCollapsed()
        {
            var result = _subject.Convert(true, null, null, null);

            result.Should().Be(Visibility.Hidden);
        }

        [Test]
        public void Convert_WhenValueIsFalse_ReturnsVisible()
        {
            var result = _subject.Convert(false, null, null, null);

            result.Should().Be(Visibility.Visible);
        }
    }
}
