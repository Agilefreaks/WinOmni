namespace OmniCommonTests.Services
{
    using System.Collections.Generic;
    using System.IO;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Domain;
    using OmniCommon.Interfaces;

    [TestFixture]
    public class SystemXmlSerializerTests
    {
        private SystemXmlSerializer _subject;

        [SetUp]
        public void SetUp()
        {
            _subject = new SystemXmlSerializer();
        }

        [Test]
        public void Deserialize_Always_ReturnsSameDataAsSerialize()
        {
            var instance = new List<string> { "str1", "str2", "str3" };
            var stream = new MemoryStream();
            _subject.Serialize(stream, instance);
            
            var result = _subject.Deserialize<List<string>>(stream);

            result.Should().Contain("str1", "str2", "str3");
        }

        [Test]
        public void Deserialize_Always_ReturnsClips()
        {
            var clip1 = new Clipping("clip1");
            var clip2 = new Clipping("clip2");
            var instance = new List<Clipping> { clip1, clip2 };
            var stream = new MemoryStream();
            _subject.Serialize(stream, instance);

            var result = _subject.Deserialize<List<Clipping>>(stream);

            result.Should()
                  .Contain(clipping => clipping.Content == "clip1")
                  .And.Contain(clipping => clipping.Content == "clip2");
        }
    }
}
