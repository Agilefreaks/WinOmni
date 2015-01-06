namespace Omnipaste.ActivityDetails.Clipping
{
    using Clipboard.Handlers.WindowsClipboard;
    using Ninject;

    public class ClippingDetailsHeaderViewModel : ActivityDetailsHeaderViewModel, IClippingDetailsHeaderViewModel
    {
        #region Public Properties

        [Inject]
        public IWindowsClipboardWrapper WindowsClipboardWrapper { get; set; }

        #endregion

        #region Public Methods and Operators

        public void CopyClipping()
        {
            WindowsClipboardWrapper.SetData(Model.Content);
        }

        #endregion
    }
}