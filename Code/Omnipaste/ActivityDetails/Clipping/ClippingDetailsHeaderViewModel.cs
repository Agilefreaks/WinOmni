namespace Omnipaste.ActivityDetails.Clipping
{
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    public class ClippingDetailsHeaderViewModel : ActivityDetailsHeaderViewModel, IClippingDetailsHeaderViewModel
    {
        #region Fields

        private ClippingDetailsHeaderStateEnum _state;

        #endregion

        #region Public Properties

        public ClippingDetailsHeaderStateEnum State
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

        [Inject]
        public IClippingRepository ClippingRepository { get; set; }

        #endregion

        #region Public Methods and Operators

        public void DeleteClipping()
        {
            Model.BackingModel.IsDeleted = true;
            ClippingRepository.Save(Model.BackingModel as ClippingModel).RunToCompletion();
            State = ClippingDetailsHeaderStateEnum.Deleted;
        }

        public void UndoDelete()
        {
            Model.BackingModel.IsDeleted = false;
            ClippingRepository.Save(Model.BackingModel as ClippingModel).RunToCompletion();
            State = ClippingDetailsHeaderStateEnum.Normal;
        }

        protected override void OnActivate()
        {
            State = ClippingDetailsHeaderStateEnum.Normal;
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            if (Model.BackingModel.IsDeleted)
            {
                ClippingRepository.Delete(Model.SourceId).RunToCompletion();
            }

            base.OnDeactivate(close);
        }

        #endregion
    }
}