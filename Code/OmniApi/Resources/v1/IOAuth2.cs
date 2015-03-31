namespace OmniApi.Resources.v1
{
    using System;
    using OmniApi.Dto;

    public interface IOAuth2
    {
        IObservable<TokenDto> Create(string authorizationCode);

        IObservable<TokenDto> Refresh(string refreshToken);
    }
}