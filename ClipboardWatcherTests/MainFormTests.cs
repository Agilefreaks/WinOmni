using System;
using System.Windows.Forms;
using ClipboardWatcher;
using ClipboardWrapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ClipboardWatcherTests
{
    [TestFixture]
    public class MainFormTests
    {
        private class MainFormWrapper : MainForm
        {
            public void CallWndProc(Message message)
            {
                base.WndProc(ref message);
            }

            public void CallOnHandleCreated()
            {
                OnHandleCreated(new EventArgs());
            }
        }

        MainFormWrapper _subject;
        private Mock<IClipboardWrapper> _mockClipboardWrapper;

        [SetUp]
        public void Setup()
        {
            _mockClipboardWrapper = new Mock<IClipboardWrapper>();
            _subject = new MainFormWrapper
                {
                    ClipboardWrapper = _mockClipboardWrapper.Object
                };
        }

        [Test]
        public void OnActivate_Always_ShouldSetTheNotifyIconVisible()
        {
            _subject.Activate();

            _subject.IsNotificationIconVisible.Should().BeTrue();
        }

        [Test]
        public void WndProc_Always_ShouldCallClipboardWrapperHandleClipboardMessage()
        {
            var message = new Message();
            _subject.CallWndProc(message);

            _mockClipboardWrapper.Verify(cw => cw.HandleClipboardMessage(message), Times.Once());
        }
    }
}
