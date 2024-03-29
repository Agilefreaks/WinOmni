﻿namespace OmnipasteTests.Notifications.NotificationList.Notification.HyperlinkNotification
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Notifications.NotificationList.Notification.HyperlinkNotification;

    public class HyperlinkNotificationViewModelTests
    {
        private HyperlinkNotificationViewModel _subject;

        private ClippingEntity _clipping;

        [SetUp]
        public void Setup()
        {
            _clipping = new ClippingEntity { Content = "test" };
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