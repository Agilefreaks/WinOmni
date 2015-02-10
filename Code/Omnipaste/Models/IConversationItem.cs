namespace Omnipaste.Models
{
    public interface IConversationItem : IModel
    {
        string Id { get; set; }

        ContactInfo ContactInfo { get; set; }

        SourceType Source { get; }

        string Content { get; }
    }
}