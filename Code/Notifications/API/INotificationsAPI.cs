 namespace Notifications.Api
{
    using System.Threading.Tasks;
    using RestSharp;
    using Retrofit.Net.Attributes.Methods;

    public interface INotificationsApi
    {
        [Get("notifications")]
        Task<IRestResponse<Models.Notification>> Last();
    }
}