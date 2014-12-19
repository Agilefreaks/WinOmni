namespace Omnipaste.DetailsViewModel
{
    using System;
    using Omnipaste.Models;
    using OmniUI.Models;

    public interface IConversationItem
    {
        ContactInfo ContactInfo { get; }

        SourceType Source { get; }

        DateTime Time { get; }
    }
}