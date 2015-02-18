namespace Omnipaste.WorkspaceDetails.Clipping
{
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;

    public class ClippingDetailsHeaderViewModel : WorkspaceDetailsHeaderViewModel<ClippingPresenter>, IClippingDetailsHeaderViewModel
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
            Model.IsDeleted = true;
            ClippingRepository.Save(Model.BackingModel).RunToCompletion();
            State = ClippingDetailsHeaderStateEnum.Deleted;
        }

        public void UndoDelete()
        {
            Model.IsDeleted = false;
            ClippingRepository.Save(Model.BackingModel).RunToCompletion();
            State = ClippingDetailsHeaderStateEnum.Normal;
        }

        protected override void OnActivate()
        {
            State = ClippingDetailsHeaderStateEnum.Normal;
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            if (Model.IsDeleted)
            {
                ClippingRepository.Delete(Model.BackingModel.UniqueId).RunToCompletion();
            }

            base.OnDeactivate(close);
        }

        #endregion
    }
}