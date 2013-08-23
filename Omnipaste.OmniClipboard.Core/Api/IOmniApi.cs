namespace Omnipaste.OmniClipboard.Core.Api
{
    using Omnipaste.OmniClipboard.Core.Api.Resources;

    public interface IOmniApi
    {
        IClippings Clippings { get; }
        string ApiKey { get; set; }
    }
}