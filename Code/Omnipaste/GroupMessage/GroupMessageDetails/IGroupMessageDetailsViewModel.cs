namespace Omnipaste.GroupMessage.GroupMessageDetails
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Omnipaste.Presenters;

    public interface IGroupMessageDetailsViewModel : IScreen
    {
        ObservableCollection<ContactInfoPresenter> Recipients { get; set; }
    }
}