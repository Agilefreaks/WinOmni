using System.Threading.Tasks;
using OmniCommon.Models;

namespace OmniApi.Resources
{
    using RestSharp;
    using Retrofit.Net.Attributes.Methods;
    using Retrofit.Net.Attributes.Parameters;

    public interface IDevicesAPI
    {
        [Post("devices/")]
        Task<IRestResponse<Device>> Register([Body] string id);

        [Put("devices/activate")]
        Task<IRestResponse<Device>> Activate([Header("Channel")] string channel, [Body] string id, [Body] string registrationId, [Body] string provider);

        [Put("devices/deactivate")]
        Task<IRestResponse<Device>> Deactivate([Body] string id);
    }
}