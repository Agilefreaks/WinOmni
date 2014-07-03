namespace Omnipaste.Dialog
{
    using System;
    using Caliburn.Micro;

    public interface IDialogViewModel : IConductActiveItem
    {
        #region Public Events

        event EventHandler<EventArgs> Closed;

        #endregion

        #region Public Properties

        bool IsOpen { get; set; }

        #endregion
    }
}