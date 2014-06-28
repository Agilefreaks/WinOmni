namespace Notifications.Api.Resources.v1
{
    using System;
    using global::Notifications.Models;
    using OmniApi.Resources;
    using Refit;

    public class Notifications : Resource<Notifications.INotificationsApi>, INotifications
    {
        #region Interfaces

        public interface INotificationsApi
        {
            #region Public Methods and Operators

            [Get("/notifications")]
            IObservable<Notification> Last([Header("Authorization")] string token);

            #endregion
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<Notification> Last()
        {
            return Authorize(ResourceApi.Last(AccessToken));
        }

        #endregion
    }
}