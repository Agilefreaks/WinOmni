using Caliburn.Micro;
using OmniCommon.Framework;

namespace Omnipaste.Dialog
{
    public interface IDialogViewModel : IWorkspace
    {
        IScreen Content { get; set; }
    }
}