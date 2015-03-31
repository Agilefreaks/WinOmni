namespace OmnipasteTests.ClippingList
{
    using FluentAssertions;
    using Microsoft.Practices.ServiceLocation;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.ClippingList;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Presenters;

    [TestFixture]
    public class ClippingViewModelFactoryTests
    {
        private ClippingViewModelFactory _subject;

        private Mock<IServiceLocator> _mockServiceLocator;

        [SetUp]
        public void SetUp()
        {
            _mockServiceLocator = new Mock<IServiceLocator>();

            _subject = new ClippingViewModelFactory(_mockServiceLocator.Object);
        }

        [Test]
        public void Create_Always_AssignsModelOnResult()
        {
            var clipping = new ClippingPresenter(new ClippingEntity());
            var mockClippingViewModel = new Mock<IClippingViewModel>();
            _mockServiceLocator.Setup(m => m.GetInstance<IClippingViewModel>())
                .Returns(mockClippingViewModel.Object);

            var result = _subject.Create(clipping);

            result.Should().Be(mockClippingViewModel.Object);
            mockClippingViewModel.VerifySet(m => m.Model = clipping);
        }
    }
}
