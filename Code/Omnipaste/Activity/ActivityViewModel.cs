namespace Omnipaste.Activity
{
    using Omnipaste.Activity.Models;
    using Omnipaste.DetailsViewModel;

    public class ActivityViewModel : DetailsViewModelBase<Models.Activity>, IActivityViewModel
    {
        #region Fields

        private ActivityViewModelStateEnum _state;

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

        #region Methods

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