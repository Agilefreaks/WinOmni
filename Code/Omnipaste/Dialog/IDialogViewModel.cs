namespace Omnipaste.Dialog
{
    using System;
    using Caliburn.Micro;

    public interface IDialogViewModel : IConductor
    {
        #region Public Events

        event EventHandler<DialogClosedEventArgs> Closed;

        #endregion

        #region Public Properties

        bool IsOpen { get; set; }

        #endregion
    }
}