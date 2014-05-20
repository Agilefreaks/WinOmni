using Notifications.Models;
using Notifications.NotificationList;
using NUnit.Framework;
using Caliburn.Micro;
using Moq;

namespace NotificationsTests
{
    [TestFixture]
    public class NoificationListViewModelTests
    {
        [Test]
        public void HandleNotification_AddsNewViewModelInTheList()
        {
            var mockEventAggregator = new Mock<IEventAggregator>();
            INotificationListViewModel subject = new NotificationListViewModel(mockEventAggregator.Object);

            subject.Handle(new Notification());

            CollectionAssert.IsNotEmpty(subject.Notifications);
        }
    }
}
