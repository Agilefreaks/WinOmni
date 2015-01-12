namespace Omnipaste.ActivityDetails.Clipping
{
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    public class ClippingDetailsHeaderViewModel : ActivityDetailsHeaderViewModel, IClippingDetailsHeaderViewModel
    {
        #region Fields

        private ClippingDetailsHeaderStateEnum _state;

        private ClippingModel _deletedClipping;

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
            ClippingRepository.Get(Model.SourceId).Where(item => item != null).SubscribeAndHandleErrors(
                item =>
                    {
                        _deletedClipping = item;
                        ClippingRepository.Delete(Model.SourceId);
                        State = ClippingDetailsHeaderStateEnum.Deleted;
                    });
        }

        public void UndoDelete()
        {
            if (_deletedClipping != null)
            {
                ClippingRepository.Save(_deletedClipping);
            }
            State = ClippingDetailsHeaderStateEnum.Normal;
        }

        protected override void OnActivate()
        {
            State = ClippingDetailsHeaderStateEnum.Normal;
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            _deletedClipping = null;
            base.OnDeactivate(close);
        }

        #endregion
    }
}