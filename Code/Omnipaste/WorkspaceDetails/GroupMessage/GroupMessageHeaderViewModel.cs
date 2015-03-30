namespace Omnipaste.WorkspaceDetails.GroupMessage{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Omnipaste.Presenters;
    
    // TODO: fix
    public class GroupMessageHeaderViewModel : WorkspaceDetailsHeaderViewModel<ContactInfoPresenter>, IGroupMessageHeaderViewModel
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