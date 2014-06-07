using System.Threading.Tasks;
using Device = OmniApi.Models.Device;

namespace OmniApi.Resources
{
    using RestSharp;
    using Retrofit.Net.Attributes.Methods;
    using Retrofit.Net.Attributes.Parameters;

    public interface IDevicesApi
    {
        [Put("devices/activate")]
        Task<IRestResponse<Device>> Activate(
            [Query("registration_id")] string registrationId, 
            [Query("identifier")] string identifier,
            [Query("provider")] string provider);

        [Post("devices/")]
        Task<IRestResponse<Device>> Register([Query("identifier")] string id, [Query("name")] string name);
        
        [Put("devices/deactivate")]
        Task<IRestResponse<Device>> Deactivate([Body] string id);
    }
}