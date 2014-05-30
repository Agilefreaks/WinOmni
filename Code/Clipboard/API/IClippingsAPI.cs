namespace Clipboard.API
{
    using System.Threading.Tasks;
    using Clipboard.Models;
    using RestSharp;
    using Retrofit.Net.Attributes.Methods;
    using Retrofit.Net.Attributes.Parameters;

    public interface IClippingsApi
    {
        [Get("clippings/last")]
        Task<IRestResponse<Clipping>> Last();

        [Post("clippings/")]
        Task<IRestResponse<Clipping>> PostClipping([Query("identifier")] string deviceIdentifier, [Query("content")]string clippingContent);
    }
}