﻿namespace Omnipaste.Loading
{
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.UserToken;

    public interface ILoadingViewModel : IScreen, IHandle<GetTokenFromUserMessage>, IHandle<TokenRequestResultMessage>
    {
        LoadingViewModelStateEnum State { get; set; }

        IUserTokenViewModel UserTokenViewModel { get; set; }
    }
}