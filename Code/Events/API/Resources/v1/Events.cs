namespace Events.Api.Resources.v1
{
    using System;
    using global::Events.Models;
    using OmniApi.Resources;
    using Refit;

    public class Events : Resource<Events.IEventsApi>, IEvents
    {
        #region Interfaces

        public interface IEventsApi
        {
            #region Public Methods and Operators

            [Get("/notifications")]
            IObservable<Event> Last([Header("Authorization")] string token);

            #endregion
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<Event> Last()
        {
            return Authorize(ResourceApi.Last(AccessToken));
        }

        #endregion
    }
}