namespace OmnipasteTests.Activity
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Activity;
    using Omnipaste.Activity.Models;

    [TestFixture]
    public class ActivityViewModelFactoryTests
    {
        private ActivityViewModelFactory _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new ActivityViewModelFactory();
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

            var activityViewModel = _subject.Create(activity);

            activityViewModel.Should().BeOfType<ContactRelatedActivityViewModel>();
            activityViewModel.Model.Should().Be(activity);
        }

        [Test]
        public void Create_ActivityIsOfTypeMessage_ReturnsAContactRelatedActivityViewModelWithTheGivenActivityAsTheModel()
        {
            var activity = new Activity(ActivityTypeEnum.Message);

            var activityViewModel = _subject.Create(activity);

            activityViewModel.Should().BeOfType<ContactRelatedActivityViewModel>();
            activityViewModel.Model.Should().Be(activity);
        }
    }
}