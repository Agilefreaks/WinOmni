namespace OmniApi.Resources.v1
{
    using System;
    using System.Net.Http;
    using OmniApi.Models;
    using OmniCommon.Interfaces;
    using Refit;

    public class Contacts : ResourceWithAuthorization<IContactsApi>, IContacts
    {
        #region Constructors and Destructors

        public Contacts(IConfigurationService configurationService, IWebProxyFactory webProxyFactory)
            : base(configurationService, webProxyFactory)
        {
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<ContactList> Get()
        {
            return ResourceApi.Get(AccessToken);
        }

        public IObservable<EmptyModel> Sync()
        {
            return ResourceApi.Sync(AccessToken);
        }

        #endregion

        #region Methods

        protected override IContactsApi CreateResourceApi(HttpClient httpClient)
        {
            return RestService.For<IContactsApi>(httpClient);
        }

        #endregion
    }
}