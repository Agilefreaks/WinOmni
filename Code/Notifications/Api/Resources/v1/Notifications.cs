namespace Notifications.Api.Resources.v1
{
    using System;
    using global::Notifications.Models;
    using OmniApi.Resources;
    using Refit;

    public class Notifications : Resource<Notifications.INotificationsApi>, INotifications
    {
        #region Interfaces

        [Headers("Authorization: bearer Xqe3xDiWlkhxmofv/1t6JLwv7awmKjvinEHOlxS028GQCHOBd0Vsokh6jioORPYStwFA2m7e17ndnbb5rnsUyA==")]
        public interface INotificationsApi
        {
            #region Public Methods and Operators

            [Get("/notifications")]
            IObservable<Notification> Last();

            #endregion
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<Notification> Last()
        {
            return ResourceApi.Last();
        }

        #endregion
    }
}