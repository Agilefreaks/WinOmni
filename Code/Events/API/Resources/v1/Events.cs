namespace Events.Api.Resources.v1
{
    using System;
    using System.Configuration;
    using global::Events.Models;
    using OmniApi.Resources;
    using OmniCommon;
    using Refit;

    public class Events : Resource<IEventsApi>, IEvents
    {
        #region Constructors and Destructors

        public Events()
            : base(CreateResourceApi())
        {
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<Event> Last()
        {
            return Authorize(ResourceApi.Last(AccessToken));
        }

        #endregion

        #region Methods

        private static IEventsApi CreateResourceApi()
        {
            return RestService.For<IEventsApi>(ConfigurationManager.AppSettings[ConfigurationProperties.BaseUrl]);
        }

        #endregion
    }
}