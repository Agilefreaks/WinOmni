using OmniCommon.Models;

namespace Clipboard
{
    using RestSharp;
    using Retrofit.Net.Attributes.Methods;
    using Retrofit.Net.Attributes.Parameters;

    public interface IClippingsAPI
    {
        [Get("clippings/last")]
        RestResponse<Clipping> LastClipping();

        [Post("clippings")]
        RestResponse<Clipping> PostClipping([Body] Clipping clipping);
    }
}