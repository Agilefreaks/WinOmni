namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Framework.Entities;

    public interface IConversationRepository : IReporsitory
    {
        IObservable<IEnumerable<ConversationEntity>> GetConversationForContact(ContactEntity contactEntity);
    }
}