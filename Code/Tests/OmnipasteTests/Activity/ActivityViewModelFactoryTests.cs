namespace OmnipasteTests.Activity
{
    using FluentAssertions;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Activity;
    using Omnipaste.Models;
    using Omnipaste.Services;
    using OmniUI.Models;

    [TestFixture]
    public class ActivityViewModelFactoryTests
    {
        private ActivityViewModelFactory _subject;

        [SetUp]
        public void Setup()
        {
            var mockingKernel = new MoqMockingKernel();
            var mockContactRelatedActivityViewModel = mockingKernel.GetMock<IContactRelatedActivityViewModel>();
            mockContactRelatedActivityViewModel.SetupAllProperties();
            var mockVersionActivityViewModel = mockingKernel.GetMock<IVersionActivityViewModel>();
            mockVersionActivityViewModel.SetupAllProperties();
            var mockActivityViewModel = mockingKernel.GetMock<IActivityViewModel>();
            mockActivityViewModel.SetupAllProperties();
            mockingKernel.Bind<IContactRelatedActivityViewModel>()
                .ToConstant(mockContactRelatedActivityViewModel.Object);
            mockingKernel.Bind<IVersionActivityViewModel>().ToConstant(mockVersionActivityViewModel.Object);
            mockingKernel.Bind<IActivityViewModel>().ToConstant(mockActivityViewModel.Object);
            _subject = new ActivityViewModelFactory { Kernel = mockingKernel };
        }

        [Test]
        public void Create_ActivityIsOfTypeClipping_ReturnsAnActivityViewModelWithTheGivenActivityAsTheModel()
        {
            var activity = new Activity(ActivityTypeEnum.Clipping);

            var activityViewModel = _subject.Create(activity);

            activityViewModel.Should().BeAssignableTo<IActivityViewModel>();
            activityViewModel.Model.Should().Be(activity);
        }

        [Test]
        public void Create_ActivityIsOfTypeCall_ReturnsAContactRelatedActivityViewModelWithTheGivenActivityAsTheModel()
        {
            var activity = new Activity(ActivityTypeEnum.Call);
            activity.ExtraData.ContactInfo = new ContactInfo();

            var activityViewModel = _subject.Create(activity);

            activityViewModel.Should().BeAssignableTo<IContactRelatedActivityViewModel>();
            activityViewModel.Model.Should().Be(activity);
        }

        [Test]
        public void Create_ActivityIsOfTypeMessage_ReturnsAContactRelatedActivityViewModelWithTheGivenActivityAsTheModel()
        {
            var activity = new Activity(ActivityTypeEnum.Message);
            activity.ExtraData.ContactInfo = new ContactInfo();

            var activityViewModel = _subject.Create(activity);

            activityViewModel.Should().BeAssignableTo<IContactRelatedActivityViewModel>();
            activityViewModel.Model.Should().Be(activity);
        }

        [Test]
        public void Create_ActivityIsOfTypeVersion_ReturnsAVersionActivityViewModelWithTheGivenActivityAsTheModel()
        {
            var activity = new Activity(ActivityTypeEnum.Version);

            var activityViewModel = _subject.Create(activity);

            activityViewModel.Should().BeAssignableTo<IVersionActivityViewModel>();
            activityViewModel.Model.Should().Be(activity);
        }
    }
}