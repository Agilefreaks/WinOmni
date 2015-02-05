namespace Omnipaste.Models
{
    public interface IConversationItem : IModel
    {
        string Id { get; set; }

        ContactInfo ContactInfo { get; }

        SourceType Source { get; }

        string Content { get; }
    }
}