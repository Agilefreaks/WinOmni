namespace NotificationsTests.Api.Resources.v1
{
    using System;
    using FluentAssertions;
    using Notifications.Api.Resources.v1;
    using Notifications.Models;
    using NUnit.Framework;

    [TestFixture]
    public class NotificationsTests
    {
        private Notifications _subject;

        [SetUp]
        public void SetUp()
        {
            _subject = new Notifications();
        }

        [Test]
        public void Last_Always_ReturnsAnObservable()
        {
            _subject.Last().Should().BeAssignableTo<IObservable<Notification>>();
        }
    }
}