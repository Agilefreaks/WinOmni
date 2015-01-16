namespace Omnipaste.Activity
{
    using System;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.ActivityDetails;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services;
    using Omnipaste.Workspaces;
    using OmniUI.ExtensionMethods;

    public class ActivityViewModel : DetailsViewModelWithAutoRefresh<ActivityPresenter>, IActivityViewModel
    {
        #region Fields

        private IActivityDetailsViewModel _detailsViewModel;

        private ContentTypeEnum _contentType;

        private ActivityContentInfo _contentInfo;

        #endregion

        #region Constructors and Destructors

        public ActivityViewModel(IUiRefreshService uiRefreshService)
            : base(uiRefreshService)
        {
            ClickCommand = new Command(ShowDetails);
        }

        #endregion

        #region Public Properties

        [Inject]
        public IActivityDetailsViewModelFactory DetailsViewModelFactory { get; set; }

        [Inject]
        public IEventAggregator EventAggregator { get; set; }

        public ICommand ClickCommand { get; set; }

        public override ActivityPresenter Model
        {
            get
            {
                return base.Model;
            }
            set
            {
                base.Model = value;
                UpdateState();
                UpdateContentInfo();
            }
        }

        public ContentTypeEnum ContentType
        {
            get
            {
                return _contentType;
            }
            set
            {
                if (value == _contentType)
                {
                    return;
                }
                _contentType = value;
                NotifyOfPropertyChange();
            }
        }

        public ActivityContentInfo ContentInfo
        {
            get
            {
                return _contentInfo;
            }
            set
            {
                if (Equals(value, _contentInfo))
                {
                    return;
                }
                _contentInfo = value;
                NotifyOfPropertyChange();
            }
        }

        public DateTime Time
        {
            get
            {
                return Model.Time;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void ShowDetails()
        {
            _detailsViewModel = _detailsViewModel ?? DetailsViewModelFactory.Create(Model);
            _detailsViewModel.Deactivated += OnDetailsClosed;
            this.GetParentOfType<IActivityWorkspace>().DetailsConductor.ActivateItem(_detailsViewModel);
            UpdateContentInfo();
            if (Model != null && !Model.WasViewed)
            {
                EventAggregator.PublishOnUIThread(new DismissNotification(Model.SourceId));
            }
        }

        #endregion

        #region Methods

        protected void OnDetailsClosed(object source, EventArgs e)
        {
            Model.WasViewed = true;
            _detailsViewModel.Deactivated -= OnDetailsClosed;
            UpdateContentInfo();
        }

        private void UpdateState()
        {
            if (Model == null)
            {
                ContentType = ContentTypeEnum.Normal;
            }
            else
            {
                switch (Model.Type)
                {
                    case ActivityTypeEnum.Call:
                        ContentType = ContentTypeEnum.Call;
                        break;
                    case ActivityTypeEnum.Message:
                        ContentType = ContentTypeEnum.Message;
                        break;
                    case ActivityTypeEnum.Version:
                        ContentType = ContentTypeEnum.Version;
                        break;
                    default:
                        ContentType = ContentTypeEnum.Normal;
                        break;
                }
            }
        }

        private void UpdateContentInfo()
        {
            if (Model == null) return;

            var contentInfo = new ActivityContentInfo { ContentType = Model.Type };
            if (_detailsViewModel != null && _detailsViewModel.IsActive)
            {
                contentInfo.ContentState = ContentStateEnum.Viewing;
            }
            else if (Model.WasViewed)
            {
                contentInfo.ContentState = ContentStateEnum.Viewed;
            }
            else
            {
                contentInfo.ContentState = ContentStateEnum.NotViewed;
            }

            ContentInfo = contentInfo;
        }

        #endregion
    }
}