namespace Omnipaste.GroupMessage.GroupMessageDetails
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Omnipaste.Presenters;
    using Omnipaste.SMSComposer;

    public class GroupMessageContentViewModel : Screen, IGroupMessageContentViewModel
    {
        private ObservableCollection<ContactInfoPresenter> _recipients;

        public GroupMessageContentViewModel(ISMSComposerViewModel smsComposer)
        {
            SMSComposer = smsComposer;
        }

        public ISMSComposerViewModel SMSComposer { get; set; }

        public ObservableCollection<ContactInfoPresenter> Recipients
        {
            get
            {
                return _recipients;
            }
            set
            {
                if (_recipients == value)
                {
                    return;
                }

                _recipients = value;
                SMSComposer.Recipients = _recipients;
                NotifyOfPropertyChange(() => Recipients);
            }
        }

        public object Model { get; set; }
    }
}