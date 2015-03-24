namespace OmnipasteTests.WorkspaceDetails
{
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Practices.ServiceLocation;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Presenters.Factories;
    using Omnipaste.Services;
    using Omnipaste.WorkspaceDetails;
    using Omnipaste.WorkspaceDetails.Clipping;
    using Omnipaste.WorkspaceDetails.Version;

    [TestFixture]
    public class WorkspaceDetailsViewModelFactoryTests
    {
        private WorkspaceDetailsViewModelFactory _subject;

        private Mock<IServiceLocator> _mockServiceLocator;

        private Mock<IPhoneCallPresenterFactory> _mockPhoneCallPresenterFactory;

        private Mock<ISmsMessagePresenterFactory> _mockSmsMessagePresenterFactory;

        private ActivityPresenterFactory _activityPresenterFactory;

        [SetUp]
        public void SetUp()
        {
            _mockServiceLocator = new Mock<IServiceLocator> { DefaultValue = DefaultValue.Mock };
            _mockPhoneCallPresenterFactory = new Mock<IPhoneCallPresenterFactory> { DefaultValue = DefaultValue.Mock };
            _mockSmsMessagePresenterFactory = new Mock<ISmsMessagePresenterFactory> { DefaultValue = DefaultValue.Mock };
            _activityPresenterFactory = new ActivityPresenterFactory(_mockPhoneCallPresenterFactory.Object, _mockSmsMessagePresenterFactory.Object);

            _subject = new WorkspaceDetailsViewModelFactory(_mockServiceLocator.Object);
        }

        [Test]
        public void CreateWithActivityPresenter_WhenActivityIsClipping_ReturnsClippingDetailsViewModel()
        {
            var result = _subject.Create(_activityPresenterFactory.Create(new ClippingModel { Content = "test" }).Wait());

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
            
            var result = _subject.Create(_activityPresenterFactory.Create(clippingModel).Wait());

            ((ClippingPresenter)result.Model).BackingModel.Should().Be(clippingModel);
        }

        [Test]
        public void CreateWithActivityPresenter_WhenActivityIsVersion_ReturnsVersionViewModelDetails()
        {
            var mockDetailsViewModel = new Mock<IVersionDetailsViewModel>();
            mockDetailsViewModel.SetupAllProperties();
            var updateInfo = new UpdateInfo();
            _mockServiceLocator.Setup(m => m.GetInstance<IVersionDetailsViewModel>()).Returns(mockDetailsViewModel.Object);

            var result = _subject.Create(_activityPresenterFactory.Create(updateInfo).Wait());

            ((UpdateInfoPresenter)result.Model).BackingModel.Should().Be(updateInfo);
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
