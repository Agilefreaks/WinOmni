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

        private ActivityViewModelStateEnum _state;

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

        public ActivityViewModelStateEnum State
        {
            get
            {
                return _state;
            }
            set
            {
                if (value == _state)
                {
                    return;
                }
                _state = value;
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
                State = ActivityViewModelStateEnum.Clipping;
            }
            else
            {
                switch (Model.Type)
                {
                    case ActivityTypeEnum.Call:
                        State = ActivityViewModelStateEnum.Call;
                        break;
                    case ActivityTypeEnum.Message:
                        State = ActivityViewModelStateEnum.Message;
                        break;
                    default:
                        State = ActivityViewModelStateEnum.Clipping;
                        break;
                }
            }
        }

        #endregion
    }
}