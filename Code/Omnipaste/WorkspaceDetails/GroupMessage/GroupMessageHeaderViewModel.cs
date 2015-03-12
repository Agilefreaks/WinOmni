namespace Omnipaste.WorkspaceDetails.GroupMessage{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Omnipaste.Presenters;
    using Omnipaste.WorkspaceDetails;

    public class GroupMessageHeaderViewModel : WorkspaceDetailsHeaderViewModel<IEnumerable<ContactInfoPresenter>>, IGroupMessageHeaderViewModel
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