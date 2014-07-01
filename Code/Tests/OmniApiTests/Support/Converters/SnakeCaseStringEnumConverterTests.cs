namespace OmniApiTests.Support.Converters
{
    using FluentAssertions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using OmniApi.Support.Converters;

    [TestFixture]
    public class SnakeCaseStringEnumConverterTests
    {
        private SnakeCaseStringEnumConverter _subject;

        private enum SomeEnum
        {
            Some,
            Other,
            SomeOther
        }

        [SetUp]
        public void SetUp()
        {
            _subject = new SnakeCaseStringEnumConverter();
        }

        [Test]
        public void CanConvert_Always_ReturnsTrueForEnums()
        {
            _subject.CanConvert(typeof(SomeEnum)).Should().BeTrue();
        }

        [Test]
        public void CanWrite_Always_ReturnsFalse()
        {
            _subject.CanWrite.Should().BeFalse();
        }

        [Test]
        public void ReadJson_WithLowerCase_ConvertsToCorectEnum()
        {
            JsonReader jsonReader = new JTokenReader(new JValue("other"));
            jsonReader.Read();

            _subject.ReadJson(jsonReader, typeof(SomeEnum), null, null).Should().Be(SomeEnum.Other);
        }

        [Test]
        public void ReadJson_WithLowerCaseAndUnderscore_ConvertsToCorectEnum()
        {
            JsonReader jsonReader = new JTokenReader(new JValue("some_other"));
            jsonReader.Read();

            _subject.ReadJson(jsonReader, typeof(SomeEnum), null, null).Should().Be(SomeEnum.SomeOther);
        }
    }
}
