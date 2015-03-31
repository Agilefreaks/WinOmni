namespace OmnipasteTests.Framework.Converters
{
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniUI.Framework.Converters;
    using OmniUI.Framework.Helpers;

    [TestFixture]
    public class StringToResourceConverterTests
    {
        private StringToResourceConverter _subject;

        private Mock<IResourceHelper> _mockResourceHelper;

        [SetUp]
        public void SetUp()
        {
            _mockResourceHelper = new Mock<IResourceHelper>();
            ResourceHelper.Instance = _mockResourceHelper.Object;
            _subject = new StringToResourceConverter();
        }

        [TearDown]
        public void TearDown()
        {
            ResourceHelper.Instance = null;
        }

        [Test]
        public void Convert_WhenValueIsString_ReturnsResourceHelperResult()
        {
            var resource = new object();
            _mockResourceHelper.Setup(m => m.GetByKey("testResource")).Returns(resource);

            _subject.Convert("testResource", null, null, null).Should().Be(resource);
        }

        [Test]
        public void Convert_WhenValueIsStringEmptyAndParameterIsSet_ReturnsResourceHelperResultForParameter()
        {
            var resource = new object();
            _mockResourceHelper.Setup(m => m.GetByKey("defaultResource")).Returns(resource);

            _subject.Convert(string.Empty, null, "defaultResource", null).Should().Be(resource);
        }
    }
}
