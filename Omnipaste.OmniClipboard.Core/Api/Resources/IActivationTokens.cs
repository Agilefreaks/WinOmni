namespace Omnipaste.OmniClipboard.Core.Api.Resources
{
    using OmniCommon.DataProviders;

    public interface IActivationTokens
    {
        string ApiUrl { get; }

        ActivationData Activate(string token); 
    }
}