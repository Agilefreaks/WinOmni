namespace Omnipaste.Framework.Services
{
    using System;
    using System.Threading.Tasks;

    public interface IUrlShortenerService
    {
        Task<Uri> Shorten(Uri urlToShorten);
    }
}