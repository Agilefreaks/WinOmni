namespace OmniApi.Resources.v1
{
    using System;
    using OmniApi.Models;

    public interface IOAuth2
    {
        IObservable<Token> Create(string authorizationCode);

        IObservable<Token> Refresh(string refreshToken);
    }
}