namespace Omnipaste.GroupMessage.GroupMessageDetails
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Omnipaste.Presenters;

    public class GroupMessageHeaderViewModel : Screen, IGroupMessageHeaderViewModel
    {
        private ObservableCollection<ContactInfoPresenter> _recipients;

        public ObservableCollection<ContactInfoPresenter> Recipients
        {
            get
            {
                return _recipients;
            }
            set
            {
                if (Equals(value, _recipients))
                {
                    return;
                }

                _recipients = value;
                NotifyOfPropertyChange(() => Recipients);
            }
        }
    }
}