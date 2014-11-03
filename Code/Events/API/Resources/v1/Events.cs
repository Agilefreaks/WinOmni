namespace Events.Api.Resources.v1
{
    using System;
    using System.Net.Http;
    using global::Events.Models;
    using OmniApi.Resources;
    using OmniCommon.Interfaces;
    using Refit;

    public class Events : ResourceWithAuthorization<IEventsApi>, IEvents
    {
        #region Constructors and Destructors

        public Events(IWebProxyFactory webProxyFactory)
            : base(webProxyFactory)
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

        protected override IEventsApi CreateResourceApi(HttpClient httpClient)
        {
            return RestService.For<IEventsApi>(httpClient);
        }

        #endregion
    }
}