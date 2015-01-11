namespace OmnipasteTests.ActivityDetails.Clipping
{
    using System.Reactive.Linq;
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.ActivityDetails.Clipping;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class ClippingDetailsHeaderViewModelTests
    {
        private ClippingDetailsHeaderViewModel _subject;

        private Mock<IWindowsClipboardWrapper> _mockWindowsClipboardWrapper;

        private Mock<IClippingRepository> _mockClippingRepository;

        [SetUp]
        public void Setup()
        {
            _mockWindowsClipboardWrapper = new Mock<IWindowsClipboardWrapper>();
            _mockClippingRepository = new Mock<IClippingRepository> { DefaultValue = DefaultValue.Mock };
            _subject = new ClippingDetailsHeaderViewModel
                           {
                               WindowsClipboardWrapper = _mockWindowsClipboardWrapper.Object,
                               Model = new ActivityPresenter(new Activity(new ClippingModel { UniqueId = "42" })),
                               ClippingRepository = _mockClippingRepository.Object
                           };
        }
        
        [Test]
        public void CopyClipping_ModelHasContent_CopiesTheContentToTheLocalClipboard()
        {
            const string Content = "some content";
            var clipping = new Clipping(Content);
            _subject.Model = new ActivityPresenter(new Activity(clipping));

            _subject.CopyClipping();

            _mockWindowsClipboardWrapper.Verify(x => x.SetData(Content), Times.Once());
        }

        [Test]
        public void DeleteClipping_WhenClippingExists_DeletesCurrentClipping()
        {
            _mockClippingRepository.Setup(m => m.Get("42")).Returns(Observable.Return(new ClippingModel()));

            _subject.DeleteClipping();

            _mockClippingRepository.Verify(m => m.Delete(_subject.Model.SourceId));
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
    }
}