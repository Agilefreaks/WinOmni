using Caliburn.Micro;

namespace Omnipaste.Dialog
{
    using System;

    public interface IDialogViewModel : IConductor
    {
        bool IsOpen { get; set; }

        event EventHandler<DialogClosedEventArgs> Closed;
    }
}