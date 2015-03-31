namespace Omnipaste.Models
{
    using Omnipaste.Entities;
    using OmniUI.Models;

    public interface IConversationModel : IModel
    {
        ContactModel ContactModel { get; }

        SourceType Source { get; }

        string Content { get; }

        IConversationModel SetContactModel(ContactModel contactModel);
    }
}