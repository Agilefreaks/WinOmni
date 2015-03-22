namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Models;

    public interface IConversationRepository : IReporsitory
    {
        IObservable<IEnumerable<ConversationBaseModel>> GetForContact(ContactInfo contactInfo);
    }
}