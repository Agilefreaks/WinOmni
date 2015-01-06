namespace OmnipasteTests.ActivityDetails.Clipping
{
    using Clipboard.Handlers.WindowsClipboard;
    using Clipboard.Models;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.ActivityDetails.Clipping;
    using Omnipaste.Models;
    using Omnipaste.Presenters;

    [TestFixture]
    public class ClippingDetailsHeaderViewModelTests
    {
        private ClippingDetailsHeaderViewModel _subject;

        private Mock<IWindowsClipboardWrapper> _mockWindowsClipboardWrapper;

        [SetUp]
        public void Setup()
        {
            _mockWindowsClipboardWrapper = new Mock<IWindowsClipboardWrapper>();
            _subject = new ClippingDetailsHeaderViewModel
                           {
                               WindowsClipboardWrapper = _mockWindowsClipboardWrapper.Object
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
    }
}