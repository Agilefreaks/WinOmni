namespace Omnipaste.GroupMessage.GroupMessageDetails
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Omnipaste.Presenters;

    public interface IGroupMessageContentViewModel : IScreen
    {
        ObservableCollection<ContactInfoPresenter> Recipients { get; set; }
    }
}