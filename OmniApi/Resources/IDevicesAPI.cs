using System.Threading.Tasks;

namespace OmniApi.Resources
{
    using RestSharp;
    using Retrofit.Net.Attributes.Methods;
    using Retrofit.Net.Attributes.Parameters;

    public interface IDevicesAPI
    {
        [Put("devices/activate")]
        Task<IRestResponse<IDevicesAPI>> Activate([Header("Channel")] string channel, [Body] string id, [Body] string registrationId, [Body] string provider);

        [Put("devices/deactivate")]
        Task<IRestResponse<IDevicesAPI>> Deactivate([Body] string id);
    }
}