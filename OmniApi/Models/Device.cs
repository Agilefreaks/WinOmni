using Retrofit.Net;

namespace OmniApi.Models
{
    public class Device : IResource
    {
        string registration_id { get; set; }

        string name { get; set; }

        string identifier { get; set; }
    }
}