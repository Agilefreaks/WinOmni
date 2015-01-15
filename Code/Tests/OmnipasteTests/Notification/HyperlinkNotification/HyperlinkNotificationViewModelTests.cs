namespace OmnipasteTests.Notification.HyperlinkNotification
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Models;
    using Omnipaste.Notification.HyperlinkNotification;

    public class HyperlinkNotificationViewModelTests
    {
        private HyperlinkNotificationViewModel _subject;

        private ClippingModel _clipping;

        [SetUp]
        public void Setup()
        {
            _clipping = new ClippingModel { Content = "test" };
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