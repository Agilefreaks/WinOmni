﻿namespace Omnipaste.Loading
{
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.Loading.UserToken;

    public interface ILoadingViewModel : IConductActiveItem, IHandle<GetTokenFromUserMessage>, IHandle<TokenRequestResultMessage>, IHandle<ActivationFailedMessage>, IHandle<ShowAndroidInstallGuideMessage>
    {
        LoadingViewModelStateEnum State { get; set; }

        IUserTokenViewModel UserTokenViewModel { get; set; }

        ILoadingViewModel Loading();
    }
}