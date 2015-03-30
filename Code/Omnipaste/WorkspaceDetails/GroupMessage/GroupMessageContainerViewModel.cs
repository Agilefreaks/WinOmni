namespace Omnipaste.WorkspaceDetails.GroupMessage
{
    using System;
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Omnipaste.Presenters;
    using Omnipaste.SMSComposer;

    public class GroupMessageContainerViewModel : Screen, IGroupMessageContainerViewModel
    {
        private ObservableCollection<ContactInfoPresenter> _recipients;

        public GroupMessageContainerViewModel(ISMSComposerViewModel smsComposer)
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

        public object Model
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}