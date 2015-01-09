namespace Omnipaste.Framework.Commands
{
    using System;
    using Clipboard.Handlers.WindowsClipboard;
    using Microsoft.Practices.ServiceLocation;

    public class SimulateClippingCommand : Command
    {
        #region Static Fields

        private static IWindowsClipboardWrapper _windowsClipboardWrapper;

        #endregion

        #region Constructors and Destructors

        public SimulateClippingCommand()
            : base(SimulateClipping, true)
        {
        }

        #endregion

        #region Methods

        private static void SimulateClipping(object parameter)
        {
            _windowsClipboardWrapper = _windowsClipboardWrapper
                                       ?? ServiceLocator.Current.GetInstance<IWindowsClipboardWrapper>();
            _windowsClipboardWrapper.SetData(Convert.ToString(parameter));
        }

        #endregion
    }
}