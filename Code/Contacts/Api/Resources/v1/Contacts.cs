﻿namespace Contacts.Api.Resources.v1
{
    using System;
    using System.Net.Http;
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

        public IObservable<ContactList> Get(string identifier)
        {
            return ResourceApi.Get(identifier, AccessToken);
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