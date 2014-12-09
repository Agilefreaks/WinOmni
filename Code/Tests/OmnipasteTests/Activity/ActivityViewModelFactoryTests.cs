namespace OmnipasteTests.Activity
{
    using FluentAssertions;
    using Moq;
    using Ninject;
    using NUnit.Framework;
    using Omnipaste.Activity;
    using Omnipaste.Activity.Models;
    using Omnipaste.Services;

    [TestFixture]
    public class ActivityViewModelFactoryTests
    {
        private ActivityViewModelFactory _subject;

        private Mock<IUiRefreshService> _mockUiRefreshService;

        [SetUp]
        public void Setup()
        {
            _mockUiRefreshService = new Mock<IUiRefreshService> {  DefaultValue = DefaultValue.Mock };
            _subject = new ActivityViewModelFactory(_mockUiRefreshService.Object)
                           {
                               Kernel = new Mock<IKernel>().Object
                           };
        }

        [Test]
        public void Create_ActivityIsOfTypeClipping_ReturnsAnActivityViewModelWithTheGivenActivityAsTheModel()
        {
            var activity = new Activity(ActivityTypeEnum.Clipping);

            var activityViewModel = _subject.Create(activity);

            activityViewModel.Should().BeOfType<ActivityViewModel>();
            activityViewModel.Model.Should().Be(activity);
        }

        [Test]
        public void Create_ActivityIsOfTypeCall_ReturnsAContactRelatedActivityViewModelWithTheGivenActivityAsTheModel()
        {
            var activity = new Activity(ActivityTypeEnum.Call);
            activity.ExtraData.ContactInfo = new ContactInfo();

            var activityViewModel = _subject.Create(activity);

            activityViewModel.Should().BeOfType<ContactRelatedActivityViewModel>();
            activityViewModel.Model.Should().Be(activity);
        }

        [Test]
        public void Create_ActivityIsOfTypeMessage_ReturnsAContactRelatedActivityViewModelWithTheGivenActivityAsTheModel()
        {
            var activity = new Activity(ActivityTypeEnum.Message);
            activity.ExtraData.ContactInfo = new ContactInfo();

            var activityViewModel = _subject.Create(activity);

            activityViewModel.Should().BeOfType<ContactRelatedActivityViewModel>();
            activityViewModel.Model.Should().Be(activity);
        }
    }
}