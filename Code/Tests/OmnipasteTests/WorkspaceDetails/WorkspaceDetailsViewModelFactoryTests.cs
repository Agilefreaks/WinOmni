namespace OmnipasteTests.WorkspaceDetails
{
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Practices.ServiceLocation;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Activities.ActivityDetails.Version;
    using Omnipaste.Clippings.CilppingDetails;
    using Omnipaste.Framework;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using Omnipaste.Framework.Models.Factories;
    using Omnipaste.Framework.Services.Repositories;

    [TestFixture]
    public class WorkspaceDetailsViewModelFactoryTests
    {
        private DetailsViewModelFactory _subject;

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

            _subject = new DetailsViewModelFactory(_mockServiceLocator.Object);
        }

        [Test]
        public void CreateWithActivityModel_WhenActivityIsClipping_ReturnsClippingDetailsViewModel()
        {
            var result = _subject.Create(_activityModelFactory.Create(new ClippingEntity { Content = "test" }).Wait());

            result.Should().BeAssignableTo<IClippingDetailsViewModel>();
        }

        [Test]
        public void CreateWithActivityModel_WhenActivityIsClipping_AssignsClippingModelOnResult()
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
        public void CreateWithActivityModel_WhenActivityIsVersion_ReturnsVersionViewModelDetails()
        {
            var mockDetailsViewModel = new Mock<IVersionDetailsViewModel>();
            mockDetailsViewModel.SetupAllProperties();
            var updateEntity = new UpdateEntity();
            _mockServiceLocator.Setup(m => m.GetInstance<IVersionDetailsViewModel>()).Returns(mockDetailsViewModel.Object);

            var result = _subject.Create(_activityModelFactory.Create(updateEntity).Wait());

            ((UpdateModel)result.Model).BackingEntity.Should().Be(updateEntity);
        }

        [Test]
        public void CreateWithClippingModel_Always_ReturnsClippingDetailsViewModel()
        {
            var result = _subject.Create(new ClippingEntity { Content = "test" });

            result.Should().BeAssignableTo<IClippingDetailsViewModel>();
        }

        [Test]
        public void CreateWithClippingModel_Always_AssignsClippingModelOnResult()
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
