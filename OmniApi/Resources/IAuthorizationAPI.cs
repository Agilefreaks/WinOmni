using System.Threading.Tasks;
using OmniApi.Models;
using RestSharp;
using Retrofit.Net.Attributes.Methods;
using Retrofit.Net.Attributes.Parameters;

namespace OmniApi.Resources
{
    public interface IAuthorizationAPI
    {
        [Post("oauth2/token")]
        Task<IRestResponse<ActivationModel>> Activate(
            [Query("code")] string code,
            [Query("client_id")] string clientId,
            [Query("grant_type")] string grantType = "authorization_code");
    }
}