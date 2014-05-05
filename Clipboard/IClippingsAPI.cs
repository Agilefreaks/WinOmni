using System.Threading.Tasks;
using OmniCommon.Models;
using RestSharp;
using Retrofit.Net.Attributes.Methods;
using Retrofit.Net.Attributes.Parameters;

namespace Clipboard
{
    public interface IClippingsAPI
    {
        [Get("clippings/last")]
        Task<IRestResponse<Clipping>> Last();

        [Post("clippings/")]
        Task<IRestResponse<Clipping>> PostClipping([Query("identifier")] string deviceIdentifier, [Query("content")]string clippingContent);
    }
}