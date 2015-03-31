namespace Contacts.Api.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using global::Contacts.Dto;
    using global::Contacts.Models;
    using OmniApi.Resources;
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

        public IObservable<ContactDto> Get(string id)
        {
            return ResourceApi.Get(id, AccessToken);
        }

        public IObservable<List<ContactDto>> GetUpdates(DateTime from)
        {
            return ResourceApi.GetUpdates(from.ToString("s"), AccessToken);
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