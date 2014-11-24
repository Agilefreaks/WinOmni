namespace OmnipasteTests.Notification.HyperlinkNotification
{
    using Clipboard.Models;
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Notification.HyperlinkNotification;

    public class HyperlinkNotificationViewModelTests
    {
        private HyperlinkNotificationViewModel _subject;

        private Clipping _clipping;

        [SetUp]
        public void Setup()
        {
            _clipping = new Clipping("test");
            _subject = new HyperlinkNotificationViewModel
                           {
                               Resource = _clipping
                           };
        }

        [Test]
        public void Line2_Always_IsStringEmpty()
        {
            _subject.Line2.Should().Be(string.Empty);
        }

        [Test]
        public void Line1_Always_IsStringEmpty()
        {
            _subject.Line1.Should().Be(string.Empty);
        }

        [Test]
        public void Uri_Always_IsClippingContent()
        {
            _subject.Uri.Should().Be(_clipping.Content);
        }
    }
}