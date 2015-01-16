namespace Omnipaste.DetailsViewModel
{
    using System;
    using Omnipaste.Models;
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