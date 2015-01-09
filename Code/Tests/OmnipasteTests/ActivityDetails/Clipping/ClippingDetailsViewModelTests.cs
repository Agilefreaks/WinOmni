namespace OmnipasteTests.ActivityDetails.Clipping
{
    using Caliburn.Micro;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.ActivityDetails.Clipping;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class ClippingDetailsViewModelTests
    {
        private ClippingDetailsViewModel _subject;

        private Mock<IClippingDetailsHeaderViewModel> _mockHeaderViewModel;

        private Mock<IClippingDetailsContentViewModel> _mockContentViewModel;

        private Mock<IClippingRepository> _mockClippingRepository;

        [SetUp]
        public void Setup()
        {
            _mockHeaderViewModel = new Mock<IClippingDetailsHeaderViewModel>();
            _mockContentViewModel = new Mock<IClippingDetailsContentViewModel>();
            _mockClippingRepository = new Mock<IClippingRepository>();
            _subject = new ClippingDetailsViewModel(
                _mockHeaderViewModel.Object,
                _mockContentViewModel.Object,
                _mockClippingRepository.Object);
        }

        [Test]
        public void Deactivate_ModelIsMarkedForDeletionAndCloseIsFalse_CallsParentDeactivateItemWithSelfAndCloseTrue()
        {
            var mockConductor = new Mock<IConductor>();
            _subject.Parent = mockConductor.Object;
            _subject.Model = new ActivityPresenter { MarkedForDeletion = true };
            ((IActivate)_subject).Activate();

            ((IClippingDetailsViewModel)_subject).Deactivate(false);

            mockConductor.Verify(x => x.DeactivateItem(_subject, true));
        }

        [Test]
        public void Deactivate_ModelIsMarkedForDeletionAndCloseIsTrue_RemovesItemFromStorage()
        {
            var mockConductor = new Mock<IConductor>();
            _subject.Parent = mockConductor.Object;
            const string SourceId = "someId";
            var activityPresenter = new ActivityPresenter(new Activity { SourceId = SourceId }) { MarkedForDeletion = true };
            _subject.Model = activityPresenter;

            ((IActivate)_subject).Activate();

            ((IClippingDetailsViewModel)_subject).Deactivate(true);

            _mockClippingRepository.Verify(m => m.Delete(SourceId), Times.Once());
        }

        [Test]
        public void Deactivate_ModelIsNotMarkedForDeletionAndCloseIsTrue_PublishesADeleteClippingMessage()
        {
            var mockConductor = new Mock<IConductor>();
            _subject.Parent = mockConductor.Object;
            _subject.Model = new ActivityPresenter { MarkedForDeletion = false };
            ((IActivate)_subject).Activate();

            ((IClippingDetailsViewModel)_subject).Deactivate(true);

            _mockClippingRepository.Verify(m => m.Delete(It.IsAny<object>()), Times.Never());
        }
    }
}