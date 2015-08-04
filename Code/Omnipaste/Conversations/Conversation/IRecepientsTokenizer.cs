namespace Omnipaste.Conversations.Conversation
{
    public interface IRecepientsTokenizer
    {
        void Tokenize(string tokens);

        string Tokenize();
    }
}