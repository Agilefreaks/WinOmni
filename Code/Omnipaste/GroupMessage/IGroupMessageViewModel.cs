namespace Omnipaste.GroupMessage
{
    using Caliburn.Micro;
    using Omnipaste.GroupMessage.ContactSelection;
    using Omnipaste.GroupMessage.GroupMessageDetails;

    public interface IGroupMessageViewModel : IScreen
    {
        IContactSelectionViewModel ContactSelection { get; set; }

        IGroupMessageDetailsViewModel GroupMessageDetails { get; set; }
    }
}