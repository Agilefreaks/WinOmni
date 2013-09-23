namespace Omnipaste.OmniClipboard.Core.Api.Resources
{
    using OmniCommon.DataProviders; 

    public interface IUsers
    {
        string ApiUrl { get; set; }

        ActivationData Activate(string token);
    }
}