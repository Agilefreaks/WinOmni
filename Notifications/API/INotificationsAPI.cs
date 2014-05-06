using System.Threading.Tasks;
using RestSharp;
using Retrofit.Net.Attributes.Methods;

namespace Notifications.API
{
    public interface INotificationsAPI
    {
        [Get("notifications")]
        Task<IRestResponse<Models.Notification>> Last();
    }
}