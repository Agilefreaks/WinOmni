namespace Omnipaste.Conversations.Conversation
{
    using OmniUI.Controls;

    public interface IRecepientsTokenizer
    {
        ITokenizeResult Tokenize(string text);
    }
}