﻿namespace OmnipasteTests.Clippings.ClippingList
{
    using FluentAssertions;
    using Microsoft.Practices.ServiceLocation;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Clippings.ClippingList;
    using Omnipaste.Clippings.ClippingList.Clipping;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;

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
            var clipping = new ClippingModel(new ClippingEntity());
            var mockClippingViewModel = new Mock<IClippingViewModel>();
            _mockServiceLocator.Setup(m => m.GetInstance<IClippingViewModel>())
                .Returns(mockClippingViewModel.Object);

            var result = _subject.Create(clipping);

            result.Should().Be(mockClippingViewModel.Object);
            mockClippingViewModel.VerifySet(m => m.Model = clipping);
        }
    }
}
