namespace OmniApi.Resources.v1
{
    using System;
    using OmniApi.Models;
    using Refit;

    public interface IOAuth2Api
    {
        #region Public Methods and Operators

        [Post("/oauth2/token")]
        IObservable<Token> Create([Body] AuthorizationRequest request);

        #endregion
    }
}