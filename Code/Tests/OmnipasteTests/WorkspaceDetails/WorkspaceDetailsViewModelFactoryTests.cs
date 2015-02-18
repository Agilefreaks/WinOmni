namespace OmnipasteTests.WorkspaceDetails
{
    using FluentAssertions;
    using Microsoft.Practices.ServiceLocation;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.WorkspaceDetails;
    using Omnipaste.WorkspaceDetails.Clipping;

    [TestFixture]
    public class WorkspaceDetailsViewModelFactoryTests
    {
        private WorkspaceDetailsViewModelFactory _subject;

        private Mock<IServiceLocator> _mockServiceLocator;

        [SetUp]
        public void SetUp()
        {
            _mockServiceLocator = new Mock<IServiceLocator> { DefaultValue = DefaultValue.Mock };
            _subject = new WorkspaceDetailsViewModelFactory(_mockServiceLocator.Object);
        }

        [Test]
        public void CreateWithActivityPresenter_WhenActivityIsClipping_ReturnsClippingDetailsViewModel()
        {
            var result = _subject.Create(new ActivityPresenter(new ClippingModel { Content = "test" }));

            result.Should().BeAssignableTo<IClippingDetailsViewModel>();
        }

        [Test]
        public void CreateWithActivityPresenter_WhenActivityIsClipping_AssignsClippingPresenterOnResult()
        {
            var mockDetailsViewModel = new Mock<IClippingDetailsViewModel>();
            mockDetailsViewModel.SetupAllProperties();
            var clippingModel = new ClippingModel { Content = "test" };
            _mockServiceLocator.Setup(m => m.GetInstance<IClippingDetailsViewModel>())
                .Returns(mockDetailsViewModel.Object);
            
            var result = _subject.Create(new ActivityPresenter(clippingModel));

            ((ClippingPresenter)result.Model).BackingModel.Should().Be(clippingModel);
        }

        [Test]
        public void CreateWithClippingPresenter_Always_ReturnsClippingDetailsViewModel()
        {
            var result = _subject.Create(new ClippingPresenter(new ClippingModel { Content = "test" }));

            result.Should().BeAssignableTo<IClippingDetailsViewModel>();
        }

        [Test]
        public void CreateWithClippingPresenter_Always_AssignsClippingPresenterOnResult()
        {
            var mockDetailsViewModel = new Mock<IClippingDetailsViewModel>();
            mockDetailsViewModel.SetupAllProperties();
            var presenter = new ClippingPresenter(new ClippingModel { Content = "test" });
            _mockServiceLocator.Setup(m => m.GetInstance<IClippingDetailsViewModel>())
                .Returns(mockDetailsViewModel.Object);

            var result = _subject.Create(presenter);

            result.Model.Should().Be(presenter);
        }
    }
}
