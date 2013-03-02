using ClipboardWatcher;
using FluentAssertions;
using NUnit.Framework;

namespace ClipboardWatcherTests
{
    [TestFixture]
    public class MainFormTests
    {
        MainForm _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new MainForm();
        }

        [Test]
        public void OnActivate_Always_ShouldSetTheNotifyIconVisible()
        {
            _subject.Activate();

            _subject.IsNotificationIconVisible.Should().BeTrue();
        }
    }
}
