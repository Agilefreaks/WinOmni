namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Entities;
    using Omnipaste.Models;

    public interface IConversationRepository : IReporsitory
    {
        IObservable<IEnumerable<ConversationEntity>> GetConversationForContact(ContactEntity contactEntity);
    }
}