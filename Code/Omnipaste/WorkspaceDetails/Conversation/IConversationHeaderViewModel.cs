namespace Omnipaste.WorkspaceDetails.Conversation
{
    public interface IConversationHeaderViewModel : IWorkspaceDetailsHeaderViewModel
    {
        ConversationHeaderStateEnum State { get; set; }
    }
}