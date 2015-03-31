namespace OmnipasteTests.WorkspaceDetails
{
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Practices.ServiceLocation;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Models.Factories;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails;
    using Omnipaste.WorkspaceDetails.Clipping;
    using Omnipaste.WorkspaceDetails.Version;

    [TestFixture]
    public class WorkspaceDetailsViewModelFactoryTests
    {
        private WorkspaceDetailsViewModelFactory _subject;

        private Mock<IServiceLocator> _mockServiceLocator;

        private Mock<IContactRepository> _mockContactRepository;

        private ActivityModelFactory _activityModelFactory;

        [SetUp]
        public void SetUp()
        {
            _mockServiceLocator = new Mock<IServiceLocator> { DefaultValue = DefaultValue.Mock };
            _mockContactRepository = new Mock<IContactRepository> { DefaultValue = DefaultValue.Mock };
            _mockContactRepository.Setup(m => m.Get(It.IsAny<string>())).Returns(Observable.Return(new ContactEntity()));
            _activityModelFactory = new ActivityModelFactory(_mockContactRepository.Object);

            _subject = new WorkspaceDetailsViewModelFactory(_mockServiceLocator.Object);
        }

        [Test]
        public void CreateWithActivityPresenter_WhenActivityIsClipping_ReturnsClippingDetailsViewModel()
        {
            var result = _subject.Create(_activityModelFactory.Create(new ClippingEntity { Content = "test" }).Wait());

            result.Should().BeAssignableTo<IClippingDetailsViewModel>();
        }

        [Test]
        public void CreateWithActivityPresenter_WhenActivityIsClipping_AssignsClippingPresenterOnResult()
        {
            var mockDetailsViewModel = new Mock<IClippingDetailsViewModel>();
            mockDetailsViewModel.SetupAllProperties();
            var clippingModel = new ClippingEntity { Content = "test" };
            _mockServiceLocator.Setup(m => m.GetInstance<IClippingDetailsViewModel>())
                .Returns(mockDetailsViewModel.Object);
            
            var result = _subject.Create(_activityModelFactory.Create(clippingModel).Wait());

            ((ClippingModel)result.Model).BackingEntity.Should().Be(clippingModel);
        }

        [Test]
        public void CreateWithActivityPresenter_WhenActivityIsVersion_ReturnsVersionViewModelDetails()
        {
            var mockDetailsViewModel = new Mock<IVersionDetailsViewModel>();
            mockDetailsViewModel.SetupAllProperties();
            var updateInfo = new UpdateEntity();
            _mockServiceLocator.Setup(m => m.GetInstance<IVersionDetailsViewModel>()).Returns(mockDetailsViewModel.Object);

            var result = _subject.Create(_activityModelFactory.Create(updateInfo).Wait());

            ((UpdateModel)result.Model).BackingEntity.Should().Be(updateInfo);
        }

        [Test]
        public void CreateWithClippingPresenter_Always_ReturnsClippingDetailsViewModel()
        {
            var result = _subject.Create(new ClippingEntity { Content = "test" });

            result.Should().BeAssignableTo<IClippingDetailsViewModel>();
        }

        [Test]
        public void CreateWithClippingPresenter_Always_AssignsClippingPresenterOnResult()
        {
            var mockDetailsViewModel = new Mock<IClippingDetailsViewModel>();
            mockDetailsViewModel.SetupAllProperties();
            var model = new ClippingEntity { Content = "test" };
            _mockServiceLocator.Setup(m => m.GetInstance<IClippingDetailsViewModel>()).Returns(mockDetailsViewModel.Object);

            var result = _subject.Create(model);

            ((ClippingModel)result.Model).BackingEntity.Should().Be(model);
        }
    }
}
