namespace Omnipaste.Models
{
    using System;
    using OmniUI.Models;

    public interface IConversationItem
    {
        string Id { get; set; }

        ContactInfo ContactInfo { get; }

        SourceType Source { get; }

        string Content { get; }

        DateTime Time { get; }

        string UniqueId { get; }
    }
}