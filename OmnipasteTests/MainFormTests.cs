using System.Windows.Forms;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Omnipaste;
using PubNubClipboard;

namespace OmnipasteTests
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
        }

        MainFormWrapper _subject;
        private Mock<IPubNubClipboard> _mockOmniclipboard;

        [SetUp]
        public void Setup()
        {
            _mockOmniclipboard = new Mock<IPubNubClipboard> { DefaultValue = DefaultValue.Mock };
            _subject = new MainFormWrapper
                {
                    OmniClipboard = _mockOmniclipboard.Object,
                };
        }

        [Test]
        public void OnActivate_Always_ShouldSetTheNotifyIconVisible()
        {
            _subject.Activate();

            _subject.IsNotificationIconVisible.Should().BeTrue();
        }

        [Test]
        public void WndProc_Always_ShouldRaiseHandleClipboardMessage()
        {
            var message = new Message();
            var callCount = 0;
            _subject.HandleClipboardMessage += (ref Message status) =>
                {
                    callCount++;
                    status.Should().Be(message);
                    return true;
                };

            _subject.CallWndProc(message);

            callCount.Should().Be(1);
        }
    }
}
