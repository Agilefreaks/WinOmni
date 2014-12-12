namespace Omnipaste.Activity
{
    using System.Windows.Input;
    using Ninject;
    using Omnipaste.ActivityDetails;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Models;
    using Omnipaste.Services;
    using Omnipaste.Workspaces;

    public class ActivityViewModel : DetailsViewModelWithAutoRefresh<Models.Activity>, IActivityViewModel
    {
        #region Fields

        private ContentTypeEnum _contentType;

        private IActivityDetailsViewModel _detailsViewModel;

        private bool _isDetailsOpen;

        #endregion

        #region Constructors and Destructors

        public ActivityViewModel(IUiRefreshService uiRefreshService)
            : base(uiRefreshService)
        {
            ClickCommand = new Command(ShowDetails);
        }

        #endregion

        #region Public Properties

        public ICommand ClickCommand { get; set; }

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

        [Inject]
        public IActivityDetailsViewModelFactory DetailsViewModelFactory { get; set; }

        public override Models.Activity Model
        {
            get
            {
                return base.Model;
            }
            set
            {
                base.Model = value;
                UpdateState();
            }
        }

        public bool IsDetailsOpen
        {
            get
            {
                return _isDetailsOpen;
            }
            set
            {
                if (value.Equals(_isDetailsOpen))
                {
                    return;
                }
                _isDetailsOpen = value;
                NotifyOfPropertyChange(() => IsDetailsOpen);
            }
        }
        #endregion

        #region Public Methods and Operators

        public void ShowDetails()
        {
            _detailsViewModel = _detailsViewModel ?? DetailsViewModelFactory.Create(Model);
            IsDetailsOpen = true;
            _detailsViewModel.Deactivated += OnDetailsClosed;
            this.GetParentOfType<IActivityWorkspace>().DetailsConductor.ActivateItem(_detailsViewModel);
        }

        #endregion

        #region Methods

        protected void OnDetailsClosed(object source, EventArgs e)
        {
            IsDetailsOpen = false;
            _detailsViewModel.Deactivated -= OnDetailsClosed;
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
                    default:
                        ContentType = ContentTypeEnum.Normal;
                        break;
                }
            }
        }

        #endregion
    }
}