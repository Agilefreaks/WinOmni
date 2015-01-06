namespace Omnipaste.ActivityDetails.Clipping
{
    using Clipboard.Handlers.WindowsClipboard;
    using Ninject;

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
        public IWindowsClipboardWrapper WindowsClipboardWrapper { get; set; }

        #endregion

        #region Public Methods and Operators

        public void CopyClipping()
        {
            WindowsClipboardWrapper.SetData(Model.Content);
        }

        public void DeleteClipping()
        {
            Model.MarkedForDeletion = true;
            State = ClippingDetailsHeaderStateEnum.Deleted;
        }

        public void UndoDelete()
        {
            Model.MarkedForDeletion = false;
            State = ClippingDetailsHeaderStateEnum.Normal;
        }

        #endregion
    }
}