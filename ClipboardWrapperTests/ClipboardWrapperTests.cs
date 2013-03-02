using System.Windows.Forms;
using ClipboardWrapper;
using ClipboardWrapper.Imports;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ClipboardWrapperTests
{
    [TestFixture]
    public class ClipboardWrapperTests
    {
        ClipboardWrapper.ClipboardWrapper _subject;
        private Mock<IClipboardAdapter> _mockClipboardAdapter;

        [SetUp]
        public void Setup()
        {
            _mockClipboardAdapter = new Mock<IClipboardAdapter>();
            _subject = new ClipboardWrapper.ClipboardWrapper
                {
                    ClipboardAdapter = _mockClipboardAdapter.Object
                };
        }

        [Test]
        public void HandleClipboardMessage_MessageIsOfTypeWM_DRAWCLIPBOARD_ItShouldReturnAResultIndicatingTheMessageHasBeenHandled()
        {
            var message = new Message { Msg = (int)Msgs.WM_DRAWCLIPBOARD };

            var clipboardMessageHandleResult = _subject.HandleClipboardMessage(message);

            clipboardMessageHandleResult.MessageHandled.Should().BeTrue();
        }

        [Test]
        public void HandleClipboardMessage_MessageIsOfTypeWM_CHANGECBCHAIN_ItShouldReturnAResultIndicatingTheMessageHasBeenHandled()
        {
            var message = new Message { Msg = (int)Msgs.WM_CHANGECBCHAIN };

            var clipboardMessageHandleResult = _subject.HandleClipboardMessage(message);

            clipboardMessageHandleResult.MessageHandled.Should().BeTrue();
        }

        [Test]
        public void HandleClipboardMessage_MessageIsOfOtherType_ItShouldReturnAResultIndicatingTheMessageHasNotBeenHandled()
        {
            var message = new Message { Msg = (int)Msgs.WM_ACTIVATE };

            var clipboardMessageHandleResult = _subject.HandleClipboardMessage(message);

            clipboardMessageHandleResult.MessageHandled.Should().BeFalse();
        }

        [Test]
        public void HandleClipboardMessage_MessageIsOfTypeWM_DRAWCLIPBOARDAndClipboardHasTextData_ItShouldReturnAResultWithTheData()
        {
            var message = new Message { Msg = (int)Msgs.WM_DRAWCLIPBOARD };
            var mockDataObject = new Mock<IDataObject>();
            _mockClipboardAdapter.Setup(x => x.GetDataObject()).Returns(mockDataObject.Object);
            mockDataObject.Setup(x => x.GetDataPresent(DataFormats.Text)).Returns(true);
            mockDataObject.Setup(x => x.GetData(DataFormats.Text)).Returns("asdasd");

            var clipboardMessageHandleResult = _subject.HandleClipboardMessage(message);

            clipboardMessageHandleResult.MessageData.Should().Be("asdasd");
        }
    }
}
