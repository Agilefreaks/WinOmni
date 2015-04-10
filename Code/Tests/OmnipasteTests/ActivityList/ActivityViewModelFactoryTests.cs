namespace OmnipasteTests.ActivityList
{
    using FluentAssertions;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.ActivityList;
    using Omnipaste.ActivityList.Activity;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;

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
            var activityModel = ActivityModel.BeginBuild(new ClippingEntity()).WithType(ActivityTypeEnum.Clipping).Build();

            var activityViewModel = _subject.Create(activityModel);

            activityViewModel.Should().BeAssignableTo<IActivityViewModel>();
            activityViewModel.Model.Should().Be(activityModel);
        }

        [Test]
        public void Create_ActivityIsOfTypeCall_ReturnsAContactRelatedActivityViewModelWithTheGivenActivityAsTheModel()
        {
            var activityModel = ActivityModel.BeginBuild(new LocalPhoneCallEntity()).WithType(ActivityTypeEnum.Call).Build();
            activityModel.ContactEntity = new ContactEntity();

            var activityViewModel = _subject.Create(activityModel);

            activityViewModel.Should().BeAssignableTo<IContactRelatedActivityViewModel>();
            activityViewModel.Model.Should().Be(activityModel);
        }

        [Test]
        public void Create_ActivityIsOfTypeMessage_ReturnsAContactRelatedActivityViewModelWithTheGivenActivityAsTheModel()
        {
            var activityModel = ActivityModel.BeginBuild(new LocalSmsMessageEntity()).WithType(ActivityTypeEnum.Message).Build();
            activityModel.ContactEntity = new ContactEntity();

            var activityViewModel = _subject.Create(activityModel);

            activityViewModel.Should().BeAssignableTo<IContactRelatedActivityViewModel>();
            activityViewModel.Model.Should().Be(activityModel);
        }

        [Test]
        public void Create_ActivityIsOfTypeVersion_ReturnsAVersionActivityViewModelWithTheGivenActivityAsTheModel()
        {
            var activityModel = ActivityModel.BeginBuild(new UpdateEntity()).WithType(ActivityTypeEnum.Version).Build();

            var activityViewModel = _subject.Create(activityModel);

            activityViewModel.Should().BeAssignableTo<IVersionActivityViewModel>();
            activityViewModel.Model.Should().Be(activityModel);
        }
    }
}