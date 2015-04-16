namespace Omnipaste.Clippings.CilppingDetails
{
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Framework.Models;
    using Omnipaste.Framework.Services.Repositories;
    using OmniUI.Details;

    public class ClippingDetailsHeaderViewModel : DetailsViewModelBase<ClippingModel>, IClippingDetailsHeaderViewModel
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
            ClippingRepository.Save(Model.BackingEntity).RunToCompletion();
            State = ClippingDetailsHeaderStateEnum.Deleted;
        }

        public void UndoDelete()
        {
            Model.IsDeleted = false;
            ClippingRepository.Save(Model.BackingEntity).RunToCompletion();
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
                ClippingRepository.Delete(Model.BackingEntity.UniqueId).RunToCompletion();
            }

            base.OnDeactivate(close);
        }

        #endregion
    }
}