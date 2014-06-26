namespace Notifications.Api.Resources.v1
{
    using System;
    using System.Configuration;
    using global::Notifications.Models;
    using Refit;

    public class Notifications : Resource, INotifications
    {
        #region Fields

        private readonly INotificationsApi _notificationsApi;

        #endregion

        #region Constructors and Destructors

        public Notifications()
        {
            _notificationsApi = RestService.For<INotificationsApi>(ConfigurationManager.AppSettings["BaseUrl"]);
        }

        #endregion

        #region Interfaces

        [Headers("Authorization: bearer Xqe3xDiWlkhxmofv/1t6JLwv7awmKjvinEHOlxS028GQCHOBd0Vsokh6jioORPYStwFA2m7e17ndnbb5rnsUyA==")]
        public interface INotificationsApi
        {
            #region Public Methods and Operators

            [Get("/api/v1/notifications")]
            IObservable<Notification> Last();

            #endregion
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<Notification> Last()
        {
            return _notificationsApi.Last();
        }

        #endregion
    }

    public abstract class Resource
    {
    }
}