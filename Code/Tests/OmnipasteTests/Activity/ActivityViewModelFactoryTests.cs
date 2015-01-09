namespace OmnipasteTests.Activity
{
    using FluentAssertions;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Activity;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
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
            var activityPresenter = new ActivityPresenter(new Activity(ActivityTypeEnum.Clipping));

            var activityViewModel = _subject.Create(activityPresenter);

            activityViewModel.Should().BeAssignableTo<IActivityViewModel>();
            activityViewModel.Model.Should().Be(activityPresenter);
        }

        [Test]
        public void Create_ActivityIsOfTypeCall_ReturnsAContactRelatedActivityViewModelWithTheGivenActivityAsTheModel()
        {
            var activityPresenter = new ActivityPresenter(new Activity(ActivityTypeEnum.Call));
            activityPresenter.ExtraData.ContactInfo = new ContactInfo();

            var activityViewModel = _subject.Create(activityPresenter);

            activityViewModel.Should().BeAssignableTo<IContactRelatedActivityViewModel>();
            activityViewModel.Model.Should().Be(activityPresenter);
        }

        [Test]
        public void Create_ActivityIsOfTypeMessage_ReturnsAContactRelatedActivityViewModelWithTheGivenActivityAsTheModel()
        {
            var activityPresenter = new ActivityPresenter(new Activity(ActivityTypeEnum.Message));
            activityPresenter.ExtraData.ContactInfo = new ContactInfo();

            var activityViewModel = _subject.Create(activityPresenter);

            activityViewModel.Should().BeAssignableTo<IContactRelatedActivityViewModel>();
            activityViewModel.Model.Should().Be(activityPresenter);
        }

        [Test]
        public void Create_ActivityIsOfTypeVersion_ReturnsAVersionActivityViewModelWithTheGivenActivityAsTheModel()
        {
            var activityPresenter = new ActivityPresenter(new Activity(ActivityTypeEnum.Version));

            var activityViewModel = _subject.Create(activityPresenter);

            activityViewModel.Should().BeAssignableTo<IVersionActivityViewModel>();
            activityViewModel.Model.Should().Be(activityPresenter);
        }
    }
}