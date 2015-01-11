namespace OmnipasteTests.ActivityDetails.Clipping
{
    using Caliburn.Micro;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.ActivityDetails.Clipping;

    [TestFixture]
    public class ClippingDetailsViewModelTests
    {
        private ClippingDetailsViewModel _subject;

        private Mock<IClippingDetailsHeaderViewModel> _mockHeaderViewModel;

        private Mock<IClippingDetailsContentViewModel> _mockContentViewModel;

        [SetUp]
        public void Setup()
        {
            _mockHeaderViewModel = new Mock<IClippingDetailsHeaderViewModel>();
            _mockContentViewModel = new Mock<IClippingDetailsContentViewModel>();
            _subject = new ClippingDetailsViewModel(
                _mockHeaderViewModel.Object,
                _mockContentViewModel.Object);
        }

        [Test]
        public void Deactivate_WasDeleted_ClosesDetailsView()
        {
            var mockConductor = new Mock<IConductor>();
            _subject.Parent = mockConductor.Object;
            _mockHeaderViewModel.SetupGet(m => m.State).Returns(ClippingDetailsHeaderStateEnum.Deleted);
            ((IActivate)_subject).Activate();

            ((IClippingDetailsViewModel)_subject).Deactivate(false);

            mockConductor.Verify(x => x.DeactivateItem(_subject, true));
        }

        [Test]
        public void Deactivate_ModelWasNotDeleted_DoesNotCloseDetailsView()
        {
            var mockConductor = new Mock<IConductor>();
            _subject.Parent = mockConductor.Object;
            _mockHeaderViewModel.SetupGet(m => m.State).Returns(ClippingDetailsHeaderStateEnum.Normal);
            ((IActivate)_subject).Activate();

            ((IClippingDetailsViewModel)_subject).Deactivate(true);

            mockConductor.Verify(x => x.DeactivateItem(_subject, true), Times.Never);
        }
    }
}