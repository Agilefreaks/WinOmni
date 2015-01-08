namespace Omnipaste.ActivityDetails.Conversation
{
    public interface IConversationHeaderViewModel : IActivityDetailsHeaderViewModel
    {
        ConversationHeaderStateEnum State { get; set; }
    }
}