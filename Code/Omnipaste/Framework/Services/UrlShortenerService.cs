namespace Omnipaste.Framework.Services
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    public class UrlShortenerService : IUrlShortenerService
    {
        #region Constants

        private const string ServiceUrl = "https://www.googleapis.com/urlshortener/v1/url";

        #endregion

        #region Fields

        private readonly WebClient _webClient = new WebClient();

        #endregion

        #region Constructors and Destructors

        public UrlShortenerService()
        {
            _webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
        }

        #endregion

        #region Public Methods and Operators

        public Task<Uri> Shorten(Uri urlToShorten)
        {
            var shortenUrlTask = _webClient.UploadStringTaskAsync(
                new Uri(ServiceUrl),
                "POST",
                string.Format(@"{{""longUrl"": ""{0}""}}", urlToShorten));

            return shortenUrlTask.ContinueWith<Uri>(OnShortenCompleted);
        }

        private static Uri OnShortenCompleted(Task<string> shortenUrlTask)
        {
            if (shortenUrlTask.IsFaulted)
            {
                throw shortenUrlTask.Exception ?? new Exception("Could not get shortened url. Check configuration");
            }

            var deserializeObject = JObject.Parse(shortenUrlTask.Result);
            var uriString = deserializeObject.GetValue("id").Value<string>();

            return new Uri(uriString);
        }

        #endregion
    }
}