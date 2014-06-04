using System.Threading.Tasks;
using Caliburn.Micro;

namespace Omnipaste.Framework
{
    public interface IDialogService
    {
        Task ShowDialog(IScreen viewModel);

        Task CloseDialog();
    }
}