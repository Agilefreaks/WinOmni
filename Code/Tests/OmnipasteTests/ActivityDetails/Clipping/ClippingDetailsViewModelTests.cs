namespace OmnipasteTests.ActivityDetails.Clipping
{
    using System;
    using Caliburn.Micro;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.ActivityDetails.Clipping;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Presenters;

    [TestFixture]
    public class ClippingDetailsViewModelTests
    {
        private ClippingDetailsViewModel _subject;

        private Mock<IClippingDetailsHeaderViewModel> _mockHeaderViewModel;

        private Mock<IClippingDetailsContentViewModel> _mockContentViewModel;

        private Mock<IEventAggregator> _mockEventAggregator;

        [SetUp]
        public void Setup()
        {
            _mockHeaderViewModel = new Mock<IClippingDetailsHeaderViewModel>();
            _mockContentViewModel = new Mock<IClippingDetailsContentViewModel>();
            _mockEventAggregator = new Mock<IEventAggregator>();
            _subject = new ClippingDetailsViewModel(
                _mockHeaderViewModel.Object,
                _mockContentViewModel.Object,
                _mockEventAggregator.Object);
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
        public void Deactivate_ModelIsMarkedForDeletionAndCloseIsTrue_PublishesADeleteClippingMessage()
        {
            var mockConductor = new Mock<IConductor>();
            _subject.Parent = mockConductor.Object;
            _subject.Model = new ActivityPresenter { MarkedForDeletion = true };
            ((IActivate)_subject).Activate();

            ((IClippingDetailsViewModel)_subject).Deactivate(true);

            _mockEventAggregator.Verify(x => x.Publish(It.IsAny<DeleteClippingMessage>(), It.IsAny<Action<Action>>()),
                Times.Once());
        }

        [Test]
        public void Deactivate_ModelIsNotMarkedForDeletionAndCloseIsTrue_PublishesADeleteClippingMessage()
        {
            var mockConductor = new Mock<IConductor>();
            _subject.Parent = mockConductor.Object;
            _subject.Model = new ActivityPresenter { MarkedForDeletion = false };
            ((IActivate)_subject).Activate();

            ((IClippingDetailsViewModel)_subject).Deactivate(true);

            _mockEventAggregator.Verify(x => x.Publish(It.IsAny<DeleteClippingMessage>(), It.IsAny<Action<Action>>()),
                Times.Never());
        }
    }
}