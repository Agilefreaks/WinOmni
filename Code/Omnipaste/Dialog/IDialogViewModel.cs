using Caliburn.Micro;

namespace Omnipaste.Dialog
{
    public interface IDialogViewModel : IConductor
    {
        bool IsOpen { get; set; }
    }
}