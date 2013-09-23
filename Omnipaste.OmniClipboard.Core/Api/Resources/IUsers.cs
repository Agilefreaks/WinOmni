namespace Omnipaste.OmniClipboard.Core.Api.Resources
{
    using OmniCommon.DataProviders; 

    public interface IUsers
    {
        string ApiUrl { get; }

        ActivationData Activate(string token);
    }
}