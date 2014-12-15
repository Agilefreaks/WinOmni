namespace Omnipaste.DetailsViewModel
{
    using System;
    using Omnipaste.Models;

    public interface IConversationItem
    {
        ContactInfo ContactInfo { get; }

        SourceType Source { get; }

        DateTime Time { get; }
    }
}