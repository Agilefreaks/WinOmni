namespace OmniCommon.Domain
{
    using System;

    public class Clipping
    {
        public DateTime DateCreated { get; set; }

        public string Content { get; set; }

        public Clipping(string content)
        {
            Content = content;
        }
    }
}