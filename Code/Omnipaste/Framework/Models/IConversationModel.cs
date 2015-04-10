namespace Omnipaste.Framework.Models
{
    using Omnipaste.Framework.Entities;
    using OmniUI.Framework.Models;

    public interface IConversationModel : IModel
    {
        ContactModel ContactModel { get; }

        SourceType Source { get; }

        string Content { get; }

        IConversationModel SetContactModel(ContactModel contactModel);
    }
}