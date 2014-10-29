namespace Events.Api.Resources.v1
{
    using System;
    using global::Events.Models;
    using Refit;

    public interface IEventsApi
    {
        #region Public Methods and Operators

        [Get("/events")]
        IObservable<Event> Last([Header("Authorization")] string token);

        #endregion
    }
}