namespace Omnipaste.Activity
{
    using System;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Activity.Models;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Services;

    public class ActivityViewModel : DetailsViewModelBase<Models.Activity>, IActivityViewModel
    {
        #region Fields

        private readonly IDisposable _refreshSubscription;

        private ContentTypeEnum _contentType;

        #endregion

        #region Constructors and Destructors

        public ActivityViewModel()
        {
        }

        public ActivityViewModel(IUiRefreshService uiRefreshService)
        {
            _refreshSubscription = uiRefreshService.RefreshObservable.SubscribeAndHandleErrors(_ => RefreshUi());
        }

        #endregion

        #region Public Properties

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

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            if (_refreshSubscription != null)
            {
                _refreshSubscription.Dispose();
            }
        }

        #endregion

        #region Methods

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