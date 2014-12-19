namespace OmniHolidaysTests.Services
{
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using OmniHolidays.Services;
    using OmniUI.Models;

    [TestFixture]
    public class TemplateProcessingServiceTests
    {
        private TemplateProcessingService _subject;

        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void Setup()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            _subject = new TemplateProcessingService(_mockConfigurationService.Object);
        }

        [Test]
        public void Process_Always_ReplacesTagsCorrectly()
        {
            const string Template = "%Hello %ContactFirstName%, my name is %UserLastName%";
            var contactInfo = new ContactInfo { FirstName = "testA", Phone = "1234" };
            _mockConfigurationService.SetupGet(x => x.UserInfo).Returns(new UserInfo { LastName = "testC" });

            _subject.Process(Template, contactInfo).Should().Be("%Hello testA, my name is testC");
        }
    }
}