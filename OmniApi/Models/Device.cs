using System;
using Retrofit.Net;

namespace OmniApi.Models
{
    public class Device : IResource
    {
        public DateTime CreatedAt { get; set; }

        public string Identifier { get; set; }

        public string Name { get; set; }

        public string RegistrationId { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}