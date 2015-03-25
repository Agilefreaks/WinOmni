namespace Omnipaste.GroupMessage.GroupMessageDetails
{
    using System.Collections.ObjectModel;
    using Omnipaste.Presenters;
    using Omnipaste.WorkspaceDetails;
    using OmniUI.Attributes;
    using OmniUI.Details;

    [UseView(typeof(DetailsViewWithHeader))]
    public class GroupMessageDetailsViewModel : DetailsViewModelWithHeaderBase<IWorkspaceDetailsHeaderViewModel, IWorkspaceDetailsContentViewModel>, IGroupMessageDetailsViewModel
    {
        private ObservableCollection<ContactInfoPresenter> _recipients;

        public IGroupMessageHeaderViewModel HeaderViewModel { get; set; }

        public IGroupMessageContentViewModel ContentViewModel { get; set; }

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

        public GroupMessageDetailsViewModel(IGroupMessageHeaderViewModel headerViewModel, IGroupMessageContentViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
            HeaderViewModel = headerViewModel;
            ContentViewModel = contentViewModel;
        }
    }
}