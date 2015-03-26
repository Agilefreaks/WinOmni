namespace Omnipaste.GroupMessage.GroupMessageDetails
{
    using System.Collections.ObjectModel;
    using Omnipaste.Presenters;
    using Omnipaste.WorkspaceDetails;
    using OmniUI.Attributes;
    using OmniUI.Details;

    [UseView(typeof(DetailsViewWithHeader))]
    public class GroupMessageDetailsViewModel :
        DetailsViewModelWithHeaderBase<IGroupMessageHeaderViewModel, IGroupMessageContentViewModel>,
        IGroupMessageDetailsViewModel
    {
        private ObservableCollection<ContactInfoPresenter> _recipients;

        public GroupMessageDetailsViewModel(
            IGroupMessageHeaderViewModel headerViewModel,
            IGroupMessageContentViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }

        #region IGroupMessageDetailsViewModel Members

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
                HeaderViewModel.Recipients = _recipients;
                ContentViewModel.Recipients = _recipients;
                NotifyOfPropertyChange(() => Recipients);
            }
        }

        IWorkspaceDetailsContentViewModel
            IDetailsViewModelWithHeader<IWorkspaceDetailsHeaderViewModel, IWorkspaceDetailsContentViewModel>.
            ContentViewModel
        {
            get
            {
                return ContentViewModel;
            }
        }

        IWorkspaceDetailsHeaderViewModel
            IDetailsViewModelWithHeader<IWorkspaceDetailsHeaderViewModel, IWorkspaceDetailsContentViewModel>.
            HeaderViewModel
        {
            get
            {
                return HeaderViewModel;
            }
        }

        #endregion
    }
}