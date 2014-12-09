namespace Omnipaste.Activity
{
    using System;
    using System.Windows.Input;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Activity.Models;
    using Omnipaste.ActivityDetails;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Services;
    using Omnipaste.Workspaces;

    public class ActivityViewModel : DetailsViewModelBase<Models.Activity>, IActivityViewModel
    {
        #region Fields

        private readonly IUiRefreshService _uiRefreshService;

        private ContentTypeEnum _contentType;

        private IActivityDetailsViewModel _detailsViewModel;

        private IDisposable _refreshSubscription;

        #endregion

        #region Constructors and Destructors

        public ActivityViewModel()
        {
            ClickCommand = new Command(ShowDetails);
        }

        public ActivityViewModel(IUiRefreshService uiRefreshService)
            : this()
        {
            _uiRefreshService = uiRefreshService;
        }

        [Inject]
        public IActivityDetailsViewModelFactory DetailsViewModelFactory { get; set; }

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

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            DisposeUiRefreshSubscription();
        }

        public void ShowDetails()
        {
            _detailsViewModel = _detailsViewModel ?? DetailsViewModelFactory.Create(Model);
            this.GetParentOfType<IActivityWorkspace>().DetailsConductor.ActivateItem(_detailsViewModel);
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            AddUiRefreshSubscription();
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                Dispose();
            }
            base.OnDeactivate(close);
        }

        private void AddUiRefreshSubscription()
        {
            DisposeUiRefreshSubscription();
            _refreshSubscription = _uiRefreshService.RefreshObservable.SubscribeAndHandleErrors(_ => RefreshUi());
        }

        private void DisposeUiRefreshSubscription()
        {
            if (_refreshSubscription == null)
            {
                return;
            }
            _refreshSubscription.Dispose();
            _refreshSubscription = null;
        }

        private void RefreshUi()
        {
            NotifyOfPropertyChange(() => Model);
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