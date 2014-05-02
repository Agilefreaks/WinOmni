using System.Threading.Tasks;
using RestSharp;
using Retrofit.Net.Attributes.Methods;

namespace Notifications
{
    public interface INotificationsAPI
    {
        [Get("notifications")]
        Task<IRestResponse<Notification>> GetAll();
    }
}