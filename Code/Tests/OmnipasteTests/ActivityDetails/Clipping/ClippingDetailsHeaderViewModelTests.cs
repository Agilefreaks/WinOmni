namespace OmnipasteTests.ActivityDetails.Clipping
{
    using System.Reactive.Linq;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails.Clipping;

    [TestFixture]
    public class ClippingDetailsHeaderViewModelTests
    {
        private ClippingDetailsHeaderViewModel _subject;

        private Mock<IClippingRepository> _mockClippingRepository;

        [SetUp]
        public void Setup()
        {
            _mockClippingRepository = new Mock<IClippingRepository> { DefaultValue = DefaultValue.Mock };
            _subject = new ClippingDetailsHeaderViewModel
                           {
                               Model = new ActivityPresenter(new ClippingModel { UniqueId = "42" }),
                               ClippingRepository = _mockClippingRepository.Object
                           };
        }
        
        [Test]
        public void DeleteClipping_WhenClippingExists_MarksCurrentClippingAsDeleted()
        {
            _mockClippingRepository.Setup(m => m.Get("42")).Returns(Observable.Return(new ClippingModel()));

            _subject.DeleteClipping();

            _mockClippingRepository.Verify(m => m.Save(_subject.Model.BackingModel as ClippingModel));
            _subject.Model.BackingModel.IsDeleted.Should().BeTrue();
        }

        [Test]
        public void DeleteClipping_WhenClippingExists_SetsViewModelStateToDeleted()
        {
            _mockClippingRepository.Setup(m => m.Get("42")).Returns(Observable.Return(new ClippingModel()));

            _subject.DeleteClipping();

            _subject.State.Should().Be(ClippingDetailsHeaderStateEnum.Deleted);
        }

        [Test]
        public void UndoDelete_Always_SetsTheViewModelStateToNormal()
        {
            _subject.UndoDelete();

            _subject.State.Should().Be(ClippingDetailsHeaderStateEnum.Normal);
        }

        [Test]
        public void UndoDelete_WhenClippingIsMarkedAsDeleted_MarksCurrentClippingAsNotDeleted()
        {
            _mockClippingRepository.Setup(m => m.Get("42")).Returns(Observable.Return(new ClippingModel { IsDeleted = true }));

            _subject.UndoDelete();

            _mockClippingRepository.Verify(m => m.Save(_subject.Model.BackingModel as ClippingModel));
            _subject.Model.BackingModel.IsDeleted.Should().BeFalse();
        }
    }
}