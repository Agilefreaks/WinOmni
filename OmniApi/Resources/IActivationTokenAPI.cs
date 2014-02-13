using System.Threading.Tasks;
using OmniApi.Models;
using RestSharp;
using Retrofit.Net.Attributes.Methods;
using Retrofit.Net.Attributes.Parameters;

namespace OmniApi.Resources
{
    public interface IActivationTokenAPI
    {
        [Put("activation")]
        Task<IRestResponse<ActivationModel>> Activate([Header("Token")] string token);
    }
}