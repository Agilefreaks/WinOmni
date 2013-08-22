namespace OmnipasteTests.Framework.Converters
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Framework.Converters;

    [TestFixture]
    public class CropStringConverterTests
    {
        private CropStringConverter _target;

        [SetUp]
        public void TestInitialize()
        {
            _target = new CropStringConverter();
        }

        [Test]
        public void Convert_WhenParameterNotSet_SetsDefaultMaximumSize()
        {
            var str = new String('a', 200);

            var result = (string)_target.Convert(str, null, null, null);

            result.Length.Should().Be(CropStringConverter.DefaultMaximumSize + CropStringConverter.Ending.Length);
        }

        [Test]
        public void Convert_WhenParameterSet_ReturnsThatNumerOfCharactersExcludingEnding()
        {
            var str = new String('a', 200);

            var result = (string)_target.Convert(str, null, 42, null);

            result.Length.Should().Be(45);
        }

        [Test]
        public void Convert_WhenValueIsShorterThanParameterLenght_DoesNotAddEnding()
        {
            var str = new string('a', 40);

            var result = (string)_target.Convert(str, null, 42, null);

            result.Length.Should().Be(40);
        }

        [Test]
        public void Convert_Always_WillInsertSeparatorWhenNewLine()
        {
            var str = string.Concat(new string('a', 10), Environment.NewLine, new string('b', 20));

            var result = (string) _target.Convert(str, null, 40, null);

            result.Should().Be(new string('a', 10) + CropStringConverter.Ending);
        }

        [Test]
        public void Convert_WhenFirstValueIsNewLine_ReturnsEnding()
        {
            var str = string.Concat(Environment.NewLine, new string('a', 20));

            var result = (string) _target.Convert(str, null, null, null);

            result.Should().Be((CropStringConverter.Ending));
        }
    }
}
